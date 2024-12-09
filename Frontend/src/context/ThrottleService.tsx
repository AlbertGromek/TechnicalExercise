import React, { createContext, useContext, useState, ReactNode, useEffect } from "react";

interface ThrottleContextType {
  rateLimitEndTime: number | null;
  checkRateLimit: () => boolean;
  setRateLimit: () => void;
}

const ThrottleContext = createContext<ThrottleContextType | undefined>(undefined);

export const ThrottleProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [rateLimitEndTime, setRateLimitEndTime] = useState<number | null>(null);

  useEffect(() => {
    const interval = setInterval(() => {
      if (rateLimitEndTime !== null) {
        const currentTime = Date.now();
        if (currentTime >= rateLimitEndTime) {
          setRateLimitEndTime(null);
        }
      }
    }, 1000);

    return () => clearInterval(interval);
  }, [rateLimitEndTime]);

  const checkRateLimit = () => {
    const currentTime = Date.now();
    if (rateLimitEndTime !== null && currentTime < rateLimitEndTime) {
      alert(`Only one request is allowed every minute on the free tier of Azure AI (ノಠ益ಠ)ノ彡┻━┻ \n Try again in ${(rateLimitEndTime - currentTime) / 1000} seconds.`);
      return false;
    }
    return true;
  };

  const setRateLimit = () => {
    const newRateLimitEndTime = Date.now() + 60000; // 60 seconds
    setRateLimitEndTime(newRateLimitEndTime);
  };

  return (
    <ThrottleContext.Provider value={{ rateLimitEndTime, checkRateLimit, setRateLimit }}>
      {children}
    </ThrottleContext.Provider>
  );
};

export const useThrottle = (): ThrottleContextType => {
  const context = useContext(ThrottleContext);
  if (!context) {
    throw new Error("useThrottle must be used within a ThrottleProvider");
  }
  return context;
};