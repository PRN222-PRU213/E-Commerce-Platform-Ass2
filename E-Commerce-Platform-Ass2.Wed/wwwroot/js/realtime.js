// Simple SignalR helper for the site.
(() => {
  const hubUrl = "/hubs/notifications";
  const callbacks = {
    productChanged: [],
    notification: [],
    cartCountChanged: [],
    preOrderChanged: [],
    connectionChanged: [],
  };

  function updateCartCountBadge(count) {
    const safeCount = Number.isFinite(Number(count))
      ? Math.max(0, Number(count))
      : 0;
    const badges = document.querySelectorAll(".js-cart-count-badge");
    badges.forEach((badge) => {
      badge.textContent = String(safeCount);
      badge.setAttribute("data-cart-count", String(safeCount));
    });
  }

  const connection = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl)
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .configureLogging(signalR.LogLevel.Information)
    .build();

  function notifyConnection(state) {
    console.log(`[SignalR] Connection state: ${state}`);
    callbacks.connectionChanged.forEach((fn) => fn(state));
  }

  connection.onreconnecting(() => {
    console.log("[SignalR] Reconnecting...");
    notifyConnection("reconnecting");
  });

  connection.onreconnected(async () => {
    console.log("[SignalR] Reconnected!");
    notifyConnection("connected");
    // Rejoin groups vì reconnect tạo connectionId mới → server xóa khỏi group cũ
    try {
      await connection.invoke("RejoinGroups");
      console.log("[SignalR] Groups rejoined after reconnect");
    } catch (err) {
      console.error("[SignalR] Failed to rejoin groups:", err);
    }
  });

  connection.onclose(() => {
    console.log("[SignalR] Connection closed");
    notifyConnection("disconnected");
  });

  connection.on("ProductChanged", (message) => {
    console.log("[SignalR] ProductChanged received:", message);
    callbacks.productChanged.forEach((fn) => fn(message));
    document.dispatchEvent(
      new CustomEvent("rt:product-changed", { detail: message, bubbles: true }),
    );
  });

  connection.on("NotificationReceived", (message) => {
    console.log("[SignalR] NotificationReceived:", message);
    callbacks.notification.forEach((fn) => fn(message));
    document.dispatchEvent(
      new CustomEvent("rt:notification", { detail: message, bubbles: true }),
    );
  });

  connection.on("ReviewApproved", (review) => {
    console.log("[SignalR] ReviewApproved received:", review);
    document.dispatchEvent(
      new CustomEvent("rt:review-approved", { detail: review, bubbles: true }),
    );
  });

  connection.on("ReviewSubmitted", (review) => {
    console.log("[SignalR] ReviewSubmitted received:", review);
    document.dispatchEvent(
      new CustomEvent("rt:review-submitted", { detail: review, bubbles: true }),
    );
  });

  connection.on("CartCountUpdated", (count) => {
    console.log("[SignalR] CartCountUpdated received:", count);
    updateCartCountBadge(count);
    callbacks.cartCountChanged.forEach((fn) => fn(count));
    document.dispatchEvent(
      new CustomEvent("rt:cart-count-updated", {
        detail: { count },
        bubbles: true,
      }),
    );
  });

  function emitPreOrder(eventName, payload) {
    console.log(`[SignalR] ${eventName} received:`, payload);
    callbacks.preOrderChanged.forEach((fn) => fn(eventName, payload));
    document.dispatchEvent(
      new CustomEvent("rt:preorder-changed", {
        detail: { eventName, payload },
        bubbles: true,
      }),
    );
  }

  connection.on("PreOrderCreated", (payload) =>
    emitPreOrder("PreOrderCreated", payload),
  );
  connection.on("PreOrderDepositPaid", (payload) =>
    emitPreOrder("PreOrderDepositPaid", payload),
  );
  connection.on("PreOrderReadyForFinalPayment", (payload) =>
    emitPreOrder("PreOrderReadyForFinalPayment", payload),
  );
  connection.on("PreOrderPaymentCompleted", (payload) =>
    emitPreOrder("PreOrderPaymentCompleted", payload),
  );
  connection.on("PreOrderExpired", (payload) =>
    emitPreOrder("PreOrderExpired", payload),
  );

  connection.on("TicketCreated", (message) => {
    console.log("[SignalR] TicketCreated received:", message);
    document.dispatchEvent(
      new CustomEvent("rt:ticket-created", { detail: message, bubbles: true }),
    );
  });

  connection.on("TicketUpdated", (message) => {
    console.log("[SignalR] TicketUpdated received:", message);
    document.dispatchEvent(
      new CustomEvent("rt:ticket-updated", { detail: message, bubbles: true }),
    );
  });

  connection.on("TicketReplied", (message) => {
    console.log("[SignalR] TicketReplied received:", message);
    document.dispatchEvent(
      new CustomEvent("rt:ticket-replied", { detail: message, bubbles: true }),
    );
  });

  async function start() {
    if (
      connection.state === signalR.HubConnectionState.Connected ||
      connection.state === signalR.HubConnectionState.Connecting
    ) {
      console.log("[SignalR] Already connected or connecting");
      return;
    }
    // Note: removed isAuthenticated() check because HttpOnly cookies
    // are invisible to document.cookie. Auth is handled server-side.
    try {
      console.log("[SignalR] Starting connection...");
      await connection.start();
      console.log("[SignalR] Connected successfully!");
      notifyConnection("connected");
    } catch (err) {
      console.error("[SignalR] Connection failed:", err);
      // Stop retrying on auth failures (401/403)
      if (
        err &&
        err.statusCode &&
        (err.statusCode === 401 || err.statusCode === 403)
      ) {
        console.warn("[SignalR] Not authenticated, will not retry.");
        return;
      }
      notifyConnection("error");
      console.log("[SignalR] Retrying in 3 seconds...");
      setTimeout(start, 3000);
    }
  }

  window.realTime = {
    start,
    onProductChanged: (fn) => callbacks.productChanged.push(fn),
    onNotification: (fn) => callbacks.notification.push(fn),
    onCartCountChanged: (fn) => callbacks.cartCountChanged.push(fn),
    onPreOrderChanged: (fn) => callbacks.preOrderChanged.push(fn),
    onConnectionChanged: (fn) => callbacks.connectionChanged.push(fn),
    updateCartCountBadge,
    connection,
  };

  // Auto-start when DOM is ready
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", start);
  } else {
    start();
  }
})();
