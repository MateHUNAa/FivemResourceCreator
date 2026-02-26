import { useEffect, useRef } from "react";
import { noop } from "@/utils/misc";
import { useRpc } from "@matehun/rpc";

type FrameVisibleSetter = (bool: boolean) => void;

const LISTENED_KEYS = ["Escape"];

export const useExitListener = (visibleSetter: FrameVisibleSetter) => {
  const setterRef = useRef<FrameVisibleSetter>(noop);

  const rpc = useRpc();

  useEffect(() => {
    setterRef.current = visibleSetter;
  }, [visibleSetter]);

  useEffect(() => {
    const keyHandler = (e: any) => {
      if (LISTENED_KEYS.includes(e.code)) {
        setterRef.current(false);
        rpc.send("exit", {});
      }
    };

    window.addEventListener("keyup", keyHandler);

    return () => window.removeEventListener("keyup", keyHandler);
  }, []);
};
