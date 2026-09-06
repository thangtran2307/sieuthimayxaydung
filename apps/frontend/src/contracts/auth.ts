import { z } from 'zod';
import { UserRole } from './common/enums';

/** Seller registration input (mirrors the backend RegisterSellerRequest). */
export const registerSchema = z.object({
  email: z.string().email().max(255),
  password: z.string().min(8).max(128),
  displayName: z.string().min(1).max(120),
  phone: z.string().max(32).optional(),
  locationProvince: z.string().max(120).optional(),
});
export type RegisterInput = z.infer<typeof registerSchema>;

/** Email/password sign-in input. */
export const loginSchema = z.object({
  email: z.string().email(),
  password: z.string().min(1),
});
export type LoginInput = z.infer<typeof loginSchema>;

/** Account summary returned by the backend on register/login (never credentials). */
export const authUserSchema = z.object({
  id: z.string(),
  email: z.string(),
  displayName: z.string(),
  role: UserRole,
});
export type AuthUser = z.infer<typeof authUserSchema>;
