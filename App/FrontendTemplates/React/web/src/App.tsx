import { useEffect, useState } from "react";
import { isEnvBrowser } from "@/utils/misc";
import { useExitListener } from "@/hooks/useExitListener";
import { useRpc } from "@matehun/rpc";

function App() {
  const [visible, setVisibility] = useState<boolean>(
    isEnvBrowser() ? true : false,
  );

  useExitListener(setVisibility);
  const rpc = useRpc();

  rpc.register("open", () => setVisibility(true));
  rpc.register("close", () => setVisibility(false));

  useEffect(() => {
    const onLoad = async () => {
      await rpc.send("uiReady", {});
    };

    addEventListener("load", onLoad);
    return () => {
      removeEventListener("load", onLoad);
    };
  }, []);

  return (
    <div className="App">
      {visible && (
        <div className="flex min-h-screen items-center justify-center px-4">
          <div className="w-full max-w-5xl rounded-2xl bg-zinc-900 p-6 shadow-xl">
            {/*  */}
          </div>
        </div>
      )}
    </div>
  );
}

export default App;
