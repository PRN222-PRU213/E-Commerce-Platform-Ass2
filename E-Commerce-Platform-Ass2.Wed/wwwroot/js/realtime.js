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
    .build();

  function notifyConnection(state) {
    callbacks.connectionChanged.forEach((fn) => fn(state));
  }

  connection.onreconnecting(() => notifyConnection("reconnecting"));
  connection.onreconnected(() => notifyConnection("connected"));
  connection.onclose(() => notifyConnection("disconnected"));

  connection.on("ProductChanged", (message) => {
    callbacks.productChanged.forEach((fn) => fn(message));
    document.dispatchEvent(
      new CustomEvent("rt:product-changed", { detail: message }),
    );
  });

  connection.on("NotificationReceived", (message) => {
    callbacks.notification.forEach((fn) => fn(message));
    document.dispatchEvent(
      new CustomEvent("rt:notification", { detail: message }),
    );
  });

  async function start() {
    if (
      connection.state === signalR.HubConnectionState.Connected ||
      connection.state === signalR.HubConnectionState.Connecting
    ) {
      return;
    }
    try {
      await connection.start();
      notifyConnection("connected");
    } catch (err) {
      notifyConnection("error");
      setTimeout(start, 3000);
      console.error("SignalR start failed", err);
    }
  }

  window.realTime = {
    start,
    onProductChanged: (fn) => callbacks.productChanged.push(fn),
    onNotification: (fn) => callbacks.notification.push(fn),
    onConnectionChanged: (fn) => callbacks.connectionChanged.push(fn),
    connection,
  };

  // Auto-start
  start();
})();
