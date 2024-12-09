import { SwaggerException } from "../api/generated-weather-data-api-client";

export function handleApiError(error: any): void {
  if (error instanceof SwaggerException) {
    if (error.status === 429) {
      const retryAfter = error.headers?.["retry-after"];
      const retryAfterSeconds = retryAfter ? parseInt(retryAfter, 10) : null;

      if (retryAfterSeconds) {
        const retryTime = new Date(Date.now() + retryAfterSeconds * 1000);
        alert(
          `Rate limit exceeded. Try again in ${retryAfterSeconds} seconds (at ${retryTime.toLocaleTimeString()}).`
        );
      } else {
        alert("Rate limit exceeded. Please retry later.");
      }
      return;
    }

    alert(`Error: ${error.message || "Unexpected server response."}`);
  } else if (error && typeof error === "object" && "content" in error) {
    alert(error.content || "An unexpected error occurred.");
  } else {
    alert("An unexpected error occurred. Please try again.");
  }
}
