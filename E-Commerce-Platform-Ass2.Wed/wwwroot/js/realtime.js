// Simple SignalR helper for the site.
(() => {
  const hubUrl = "/hubs/notifications";
  const callbacks = {
    productChanged: [],
    notification: [],
    connectionChanged: [],
  };

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
    onConnectionChanged: (fn) => callbacks.connectionChanged.push(fn),
    connection,
  };

  // Auto-start when DOM is ready
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", start);
  } else {
    start();
  }
})();
