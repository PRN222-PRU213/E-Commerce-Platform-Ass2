(function () {
    "use strict";

    const openBtn = document.getElementById("openShopperBtn");
    const closeBtn = document.getElementById("closeShopperBtn");
    const clearBtn = document.getElementById("clearShopperBtn");
    const widget = document.getElementById("shopperWidget");
    const messagesEl = document.getElementById("shopperMessages");
    const combosEl = document.getElementById("shopperCombos");
    const inputEl = document.getElementById("shopperInput");
    const sendBtn = document.getElementById("shopperSendBtn");

    if (!openBtn || !widget) return;

    // ── localStorage persistence ───────────────────────────────────────────────
    const HISTORY_KEY = `pShopper_history_${window.__shopperUserId || "guest"}`;
    const MAX_STORED  = 60; // max messages kept in storage

    function loadHistory() {
        try {
            const raw = localStorage.getItem(HISTORY_KEY);
            return raw ? JSON.parse(raw) : [];
        } catch { return []; }
    }

    function saveHistory() {
        try {
            localStorage.setItem(HISTORY_KEY, JSON.stringify(history.slice(-MAX_STORED)));
        } catch { /* storage full or private mode */ }
    }

    function clearHistory() {
        history = [];
        localStorage.removeItem(HISTORY_KEY);
        messagesEl.innerHTML = "";
        hideCombos();
        appendMessage(
            "🗑️ Lịch sử đã được xóa.\n👋 Xin chào! Tôi là AI Personal Shopper của bạn. Hãy cho tôi biết bạn cần mua sắm gì hôm nay?",
            "assistant"
        );
    }

    // ── Initialise from saved history ──────────────────────────────────────────
    let history = loadHistory();
    let isLoading = false;

    (function initMessages() {
        messagesEl.innerHTML = "";
        if (history.length === 0) {
            appendMessage(
                "👋 Xin chào! Tôi là AI Personal Shopper của bạn.\nHãy cho tôi biết bạn cần mua sắm gì hôm nay? (ví dụ: \"Tôi cần trang phục đi phỏng vấn, ngân sách 500k\")",
                "assistant"
            );
        } else {
            history.forEach(msg =>
                appendMessage(msg.content, msg.role === "user" ? "user" : "assistant")
            );
        }
    })();

    // ── Toggle open/close ──────────────────────────────────────────────────────
    openBtn.addEventListener("click", () => {
        widget.style.display = "flex";
        widget.style.flexDirection = "column";
        openBtn.style.display = "none";
        messagesEl.scrollTop = messagesEl.scrollHeight;
        inputEl.focus();
    });

    closeBtn.addEventListener("click", () => {
        widget.style.display = "none";
        openBtn.style.display = "flex";
    });

    if (clearBtn) clearBtn.addEventListener("click", clearHistory);

    // ── Send on button click or Enter ──────────────────────────────────────────
    sendBtn.addEventListener("click", sendMessage);
    inputEl.addEventListener("keypress", (e) => {
        if (e.key === "Enter") sendMessage();
    });

    // ── CSRF token helper ──────────────────────────────────────────────────────
    function getCsrfToken() {
        return document.querySelector('meta[name="shopper-csrf"]')?.content ?? "";
    }

    // ── Core send function ─────────────────────────────────────────────────────
    async function sendMessage() {
        const text = inputEl.value.trim();
        if (!text || isLoading) return;

        inputEl.value = "";
        appendMessage(text, "user");
        history.push({ role: "user", content: text });

        showTyping();
        isLoading = true;
        sendBtn.disabled = true;

        try {
            const res = await fetch("/Api/PersonalShopper/Chat", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": getCsrfToken()
                },
                body: JSON.stringify({ message: text, history: history.slice(0, -1) })
            });

            if (!res.ok) {
                const body = await res.text().catch(() => "(no body)");
                console.error("PersonalShopper /Chat error", res.status, body);
                throw new Error("Server error " + res.status + ": " + body);
            }

            const data = await res.json();
            removeTyping();

            appendMessage(data.message, "assistant");
            history.push({ role: "assistant", content: data.message });
            saveHistory();

            if (data.combos && data.combos.length > 0) {
                renderCombos(data.combos);
            } else {
                hideCombos();
            }
        } catch (err) {
            console.error("PersonalShopper sendMessage error:", err);
            removeTyping();
            appendMessage("⚠️ " + (err.message || "Đã xảy ra lỗi khi kết nối AI. Vui lòng thử lại."), "assistant");
        } finally {
            isLoading = false;
            sendBtn.disabled = false;
        }
    }

    // ── Message rendering ──────────────────────────────────────────────────────
    function appendMessage(content, role) {
        const div = document.createElement("div");
        div.className = `shopper-msg shopper-msg--${role === "user" ? "user" : "ai"}`;

        const bubble = document.createElement("div");
        bubble.className = "shopper-bubble";
        bubble.innerHTML = escapeHtml(content).replace(/\n/g, "<br>");

        div.appendChild(bubble);
        messagesEl.appendChild(div);
        messagesEl.scrollTop = messagesEl.scrollHeight;
    }

    function showTyping() {
        const div = document.createElement("div");
        div.id = "shopperTyping";
        div.className = "shopper-msg shopper-msg--ai";
        div.innerHTML = '<div class="shopper-bubble shopper-typing">AI đang suy nghĩ...</div>';
        messagesEl.appendChild(div);
        messagesEl.scrollTop = messagesEl.scrollHeight;
    }

    function removeTyping() {
        const el = document.getElementById("shopperTyping");
        if (el) el.remove();
    }

    // ── Combo rendering ────────────────────────────────────────────────────────
    function renderCombos(combos) {
        combosEl.innerHTML = "";

        const header = document.createElement("div");
        header.style.cssText = "padding:10px 2px 6px; font-weight:600; color:#6f42c1; font-size:0.875rem;";
        header.textContent = "✨ Gợi ý bộ sản phẩm cho bạn:";
        combosEl.appendChild(header);

        combos.forEach((combo, idx) => {
            const card = document.createElement("div");
            card.className = "combo-card";

            // Header
            const cardHeader = document.createElement("div");
            cardHeader.className = "combo-card-header";
            cardHeader.innerHTML = `<strong>${escapeHtml(combo.name)}</strong><span class="style-badge">${escapeHtml(combo.style)}</span><br><small style="opacity:0.9">${escapeHtml(combo.description)}</small>`;
            card.appendChild(cardHeader);

            // Products
            const cardBody = document.createElement("div");
            cardBody.className = "combo-card-body";
            (combo.products || []).forEach(p => {
                const item = document.createElement("div");
                item.className = "combo-product-item";
                const imgSrc = p.imageUrl || "https://via.placeholder.com/44x44?text=SP";
                item.innerHTML = `
                    <img src="${escapeHtml(imgSrc)}" alt="${escapeHtml(p.name)}" class="combo-product-img" onerror="this.src='https://via.placeholder.com/44x44?text=SP'">
                    <div style="flex:1; min-width:0;">
                        <div style="font-size:0.8rem; font-weight:600; white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">${escapeHtml(p.name)}</div>
                        <div style="font-size:0.75rem; color:#6c757d;">${p.variantName ? escapeHtml(p.variantName) : ""}${p.size ? " | " + escapeHtml(p.size) : ""}${p.color ? " | " + escapeHtml(p.color) : ""}</div>
                    </div>
                    <div style="font-size:0.8rem; font-weight:600; color:#e83e8c; white-space:nowrap;">${formatPrice(p.price)}</div>`;
                cardBody.appendChild(item);
            });
            card.appendChild(cardBody);

            // Total
            const total = document.createElement("div");
            total.className = "combo-total";
            total.innerHTML = `Tổng: <span style="color:#e83e8c">${formatPrice(combo.totalPrice)}</span>`;
            card.appendChild(total);

            // Add to cart button
            const addBtn = document.createElement("button");
            addBtn.className = "btn-add-combo";
            addBtn.textContent = "🛒 Thêm cả bộ vào giỏ hàng";
            addBtn.addEventListener("click", () => handleAddCombo(combo, addBtn));
            card.appendChild(addBtn);

            combosEl.appendChild(card);
        });

        combosEl.style.display = "block";
        combosEl.style.maxHeight = "260px";
        combosEl.style.overflowY = "auto";
        combosEl.style.padding = "0 12px 12px";
    }

    function hideCombos() {
        combosEl.style.display = "none";
        combosEl.style.maxHeight = "0";
        combosEl.innerHTML = "";
    }

    // ── Add combo to cart ──────────────────────────────────────────────────────
    async function handleAddCombo(combo, btn) {
        if (!window.__shopperIsAuthenticated) {
            window.location.href = "/Authentication/Login";
            return;
        }

        const variantIds = (combo.products || []).map(p => p.variantId).filter(id => id);
        if (variantIds.length === 0) return;

        btn.disabled = true;
        btn.textContent = "Đang thêm...";

        try {
            const res = await fetch("/Api/PersonalShopper/AddCombo", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": getCsrfToken()
                },
                body: JSON.stringify({ variantIds })
            });

            if (res.status === 401) {
                window.location.href = "/Authentication/Login";
                return;
            }

            if (!res.ok) throw new Error("Server error " + res.status);

            btn.textContent = "✅ Đã thêm vào giỏ hàng!";
            btn.style.background = "#28a745";
            appendMessage(`✅ Đã thêm bộ "${combo.name}" (${variantIds.length} sản phẩm) vào giỏ hàng!`, "assistant");
        } catch {
            btn.disabled = false;
            btn.textContent = "🛒 Thêm cả bộ vào giỏ hàng";
            appendMessage("⚠️ Không thể thêm vào giỏ hàng. Vui lòng thử lại.", "assistant");
        }
    }

    // ── Helpers ────────────────────────────────────────────────────────────────
    function escapeHtml(str) {
        if (!str) return "";
        return String(str)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;");
    }

    function formatPrice(amount) {
        if (amount == null) return "—";
        return new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(amount);
    }
})();
