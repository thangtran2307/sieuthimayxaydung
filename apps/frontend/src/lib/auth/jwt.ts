/**
 * Minimal JWT payload decoding, used ONLY by the middleware to read the access token's `exp` and
 * decide when to refresh — never to authenticate or read identity (that comes from the backend's
 * `/auth/me`). No signature verification and no signing key involved. Edge- and Node-safe (uses
 * `atob` + `TextDecoder`, no Buffer).
 */

export interface JwtPayload {
  exp?: number;
  [claim: string]: unknown;
}

export function decodeJwt(token: string): JwtPayload | null {
  const payload = token.split('.')[1];
  if (!payload) {
    return null;
  }

  try {
    const base64 = payload.replaceAll('-', '+').replaceAll('_', '/');
    const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), '=');
    const binary = atob(padded);
    const bytes = Uint8Array.from(binary, (char) => char.codePointAt(0) ?? 0);
    return JSON.parse(new TextDecoder().decode(bytes)) as JwtPayload;
  } catch {
    return null;
  }
}

/** True when the token is missing an `exp` or expires within `skewSeconds` from now. */
export function isExpiring(payload: JwtPayload | null, skewSeconds: number): boolean {
  if (typeof payload?.exp !== 'number') {
    return true;
  }
  const now = Math.floor(Date.now() / 1000);
  return payload.exp - now <= skewSeconds;
}
