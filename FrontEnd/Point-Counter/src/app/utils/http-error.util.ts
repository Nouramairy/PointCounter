import { HttpErrorResponse } from '@angular/common/http';

export function getApiErrorMessage(error: unknown): string {
  if (!(error instanceof HttpErrorResponse)) {
    return 'Ett oväntat fel inträffade.';
  }

  const body = error.error;

  if (typeof body === 'string') {
    const t = body.trim();
    if (t.length > 0) {
      return t;
    }
  }

  if (body && typeof body === 'object') {
    const record = body as Record<string, unknown>;
    const errors = record['errors'];
    if (errors && typeof errors === 'object') {
      const parts: string[] = [];
      for (const value of Object.values(errors as Record<string, unknown>)) {
        if (Array.isArray(value)) {
          for (const m of value) {
            if (typeof m === 'string') {
              parts.push(m);
            }
          }
        } else if (typeof value === 'string') {
          parts.push(value);
        }
      }
      if (parts.length > 0) {
        return parts.join(' ');
      }
    }

    const detail = record['detail'];
    if (typeof detail === 'string' && detail.trim().length > 0) {
      return detail.trim();
    }

    const title = record['title'];
    if (typeof title === 'string' && title.trim().length > 0) {
      return title.trim();
    }
  }

  if (error.status === 404) {
    return 'Resursen hittades inte.';
  }
  if (error.status === 0) {
    return 'Kunde inte nå servern. Kontrollera att API:et körs.';
  }

  return `Begäran misslyckades (HTTP ${error.status}).`;
}
