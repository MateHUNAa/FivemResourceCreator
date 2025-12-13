import { useState } from "react";
import { isEnvBrowser } from "@/utils/misc";
import { useExitListener } from "@/hooks/useExitListener";
import useNuiEvent from "@/hooks/useNuiEvent";

function App() {
  const [visible, setVisibility] = useState<boolean>(isEnvBrowser() ? true : false);
  useExitListener(setVisibility);

  useNuiEvent("open", () => setVisibility(true))
  useNuiEvent("close", () => setVisibility(false))

  return (
    <div className="App">
      {visible && (
        <div className="min-h-screen flex items-center justify-center px-4">
          <div className="bg-zinc-900 p-6 rounded-2xl shadow-xl max-w-5xl w-full">
            {/*  */}
          </div>
        </div>
      )}
    </div>
  )
}

export default App
