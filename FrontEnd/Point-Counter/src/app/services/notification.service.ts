import { Injectable, signal } from '@angular/core';

export type NotificationType = 'success' | 'error' | 'info';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  readonly state = signal<{ message: string; type: NotificationType } | null>(null);

  private clearTimer: ReturnType<typeof setTimeout> | undefined;

  show(message: string, type: NotificationType = 'info'): void {
    if (this.clearTimer !== undefined) {
      clearTimeout(this.clearTimer);
    }
    this.state.set({ message, type });
    this.clearTimer = setTimeout(() => {
      this.state.set(null);
      this.clearTimer = undefined;
    }, 4500);
  }
}
