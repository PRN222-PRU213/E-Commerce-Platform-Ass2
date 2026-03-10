document.addEventListener("DOMContentLoaded", function () {
  const openBtn = document.getElementById("openChatBtn");
  const closeBtn = document.getElementById("closeChatBtn");
  const chatWidget = document.getElementById("chatWidget");
  const sendBtn = document.getElementById("chatSendBtn");
  const inputContent = document.getElementById("chatInputContent");
  const messagesContainer = document.getElementById("chatMessages");
  const shopIdInput = document.getElementById("chatShopId");

  let currentSessionId = null;
  let customerUserId = null; // Ideally passed from authenticated user logic, or handled by backend via API call.
  let isConnected = false;

  // To keep it simple, we may need to call an API first to GetOrCreateSession and get the SessionID + History.
  // For now, let's assume we invoke a REST API or backend fetch when chat is opened.

  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/chat")
    .withAutomaticReconnect()
    .build();

  connection.on("ReceiveMessage", function (message) {
    if (
      currentSessionId &&
      currentSessionId.toLowerCase() === message.chatSessionId.toLowerCase()
    ) {
      appendMessage(message.content, message.senderRole, message.createdAt);
    }
  });

  // Re-join session group after auto-reconnect (SignalR clears groups on reconnect)
  connection.onreconnected(async () => {
    if (currentSessionId) {
      try {
        await connection.invoke("JoinSession", currentSessionId);
      } catch (err) {
        console.error("Failed to re-join session after reconnect:", err);
      }
    }
  });

  // Toggle logic
  if (openBtn) {
    openBtn.addEventListener("click", async () => {
      chatWidget.style.display = "block";
      openBtn.style.display = "none";

      if (shopIdInput && !currentSessionId && !isConnected) {
        // First open: create/get session and connect
        await fetchAndConnectSession(shopIdInput.value);
      } else if (currentSessionId && isConnected) {
        // Subsequent opens: refresh history to pick up messages received while closed
        await refreshHistory();
      }
    });
  }

  if (closeBtn) {
    closeBtn.addEventListener("click", () => {
      chatWidget.style.display = "none";
      openBtn.style.display = "block";
    });
  }

  if (sendBtn) {
    sendBtn.addEventListener("click", sendMessage);
  }

  if (inputContent) {
    inputContent.addEventListener("keypress", function (e) {
      if (e.key === "Enter") {
        sendMessage();
      }
    });
  }

  async function refreshHistory() {
    try {
      const response = await fetch(
        `/Api/Chat/History?sessionId=${currentSessionId}`,
      );
      if (response.ok) {
        const messages = await response.json();
        messagesContainer.innerHTML = "";
        messages.forEach((m) =>
          appendMessage(m.content, m.senderRole, m.createdAt),
        );
      }
    } catch (err) {
      console.error("Failed to refresh chat history:", err);
    }
  }

  async function fetchAndConnectSession(shopId) {
    // Need to create an API endpoint to GET /api/chat/session/shopId
    try {
      const response = await fetch(`/Api/Chat/Session?shopId=${shopId}`);
      if (response.ok) {
        const data = await response.json();
        currentSessionId = data.sessionId;
        customerUserId = data.customerId;

        // Load history
        messagesContainer.innerHTML = "";
        if (data.history) {
          data.history.forEach((m) =>
            appendMessage(m.content, m.senderRole, m.createdAt),
          );
        }

        // Connect to SignalR
        await connection.start();
        isConnected = true;
        await connection.invoke("JoinSession", currentSessionId);
      } else if (response.status === 401) {
        messagesContainer.innerHTML =
          '<div class="text-center text-muted mt-3">Vui lòng đăng nhập để chat.</div>';
      }
    } catch (err) {
      console.error(err);
    }
  }

  async function sendMessage() {
    const content = inputContent.value.trim();
    if (content && currentSessionId && isConnected) {
      inputContent.value = "";
      try {
        // Invoke SignalR method directly
        await connection.invoke(
          "SendMessage",
          currentSessionId,
          customerUserId,
          "Customer",
          content,
          null
        );
      } catch (err) {
        console.error("Error sending message:", err);
      }
    }
  }

  function appendMessage(content, role, createdAt) {
    const msgDiv = document.createElement("div");
    msgDiv.className = `message ${role.toLowerCase()}`;

    const timeStr = new Date(createdAt).toLocaleTimeString([], {
      hour: "2-digit",
      minute: "2-digit",
    });

    msgDiv.innerHTML = `
            <div>${content}</div>
            <span class="msg-time">${timeStr}</span>
        `;
    messagesContainer.appendChild(msgDiv);
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
  }
});
