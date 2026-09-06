'use server';

import {
  errorEnvelopeSchema,
  loginSchema,
  registerSchema,
  type LoginInput,
  type RegisterInput,
} from '@/contracts';
import { API_BASE_URL, readSessionTokens } from '@/lib/auth/config';
import { clearSessionCookies, setSessionCookies } from '@/lib/auth/session';

/** Serializable result handed back to the client form (never throws across the boundary). */
export type AuthResult = { ok: true } | { ok: false; code: string; message: string };

/** Posts credentials to the backend and, on success, stores the returned session cookies. */
async function submitAuth(path: string, body: unknown): Promise<AuthResult> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    method: 'POST',
    headers: { 'content-type': 'application/json' },
    body: JSON.stringify(body),
    cache: 'no-store',
  });

  const json: unknown = await response.json().catch(() => null);

  if (!response.ok) {
    const parsed = errorEnvelopeSchema.safeParse(json);
    return parsed.success
      ? { ok: false, code: parsed.data.error.code, message: parsed.data.error.message }
      : { ok: false, code: 'INTERNAL_ERROR', message: 'Something went wrong.' };
  }

  const tokens = readSessionTokens(response.headers.getSetCookie());
  if (!tokens) {
    return { ok: false, code: 'INTERNAL_ERROR', message: 'No session was returned.' };
  }

  await setSessionCookies(tokens.access, tokens.refresh);
  return { ok: true };
}

/** Registers a new seller and starts a session. */
export async function register(input: RegisterInput): Promise<AuthResult> {
  const parsed = registerSchema.safeParse(input);
  if (!parsed.success) {
    return { ok: false, code: 'VALIDATION_FAILED', message: 'Invalid input.' };
  }

  const { email, password, displayName, phone, locationProvince } = parsed.data;
  return submitAuth('/v1/auth/register', {
    email,
    password,
    displayName,
    phone: phone || null,
    locationProvince: locationProvince || null,
  });
}

/** Signs in an existing account. */
export async function login(input: LoginInput): Promise<AuthResult> {
  const parsed = loginSchema.safeParse(input);
  if (!parsed.success) {
    return { ok: false, code: 'VALIDATION_FAILED', message: 'Invalid input.' };
  }

  return submitAuth('/v1/auth/login', parsed.data);
}

/** Ends the session by clearing the cookies on this origin. */
export async function logout(): Promise<void> {
  await clearSessionCookies();
}
