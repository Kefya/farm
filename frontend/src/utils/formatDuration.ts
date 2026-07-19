export function formatDuration(ms: number): string {
  if (ms <= 0) return '00:00';
  const totalSec = Math.floor(ms / 1000);
  const hours = Math.floor(totalSec / 3600);
  const minutes = Math.floor((totalSec % 3600) / 60);
  const seconds = totalSec % 60;

  const pad = (n: number) => n.toString().padStart(2, '0');
  if (hours > 0) {
    return '${hours}:${pad(minutes)}:${pad(seconds)}';
  }
  return '${pad(minutes)}:${pad(seconds)}';
}