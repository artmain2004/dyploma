export const getRolesFromToken = (token: string | null): string[] => {
  if (!token) {
    return [];
  }
  const parts = token.split(".");
  if (parts.length < 2) {
    return [];
  }
  try {
    const payload = JSON.parse(atob(parts[1].replace(/-/g, "+").replace(/_/g, "/")));
    const raw =
      payload["role"] ||
      payload["roles"] ||
      payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    if (Array.isArray(raw)) {
      return raw;
    }
    if (typeof raw === "string") {
      return [raw];
    }
    return [];
  } catch {
    return [];
  }
};
