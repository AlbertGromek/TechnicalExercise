// import { useState, useEffect } from "react";

// const useRateLimit = (limitDuration: number) => {
//   const [rateLimitEndTime, setRateLimitEndTime] = useState<number | null>(null);

//   useEffect(() => {
//     const interval = setInterval(() => {
//       if (rateLimitEndTime !== null) {
//         const currentTime = Date.now();
//         if (currentTime >= rateLimitEndTime) {
//           setRateLimitEndTime(null);
//         }
//       }
//     }, 1000);

//     return () => clearInterval(interval);
//   }, [rateLimitEndTime]);

//   const checkRateLimit = () => {
//     const currentTime = Date.now();
//     if (rateLimitEndTime !== null && currentTime < rateLimitEndTime) {
//       alert(`Try again in ${(rateLimitEndTime - currentTime) / 1000} seconds due to Azure Free Tier rate limit.`);
//       return false;
//     }
//     return true;
//   };

//   const setRateLimit = () => {
//     const newRateLimitEndTime = Date.now() + limitDuration;
//     setRateLimitEndTime(newRateLimitEndTime);
//   };

//   return { checkRateLimit, setRateLimit, rateLimitEndTime };
// };

// export default useRateLimit;