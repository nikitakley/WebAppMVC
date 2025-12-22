export const isValidEmail = (email) => {
    if (!email) return true;
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(String(email).toLowerCase());
};

export const isValidPhone = (phone) => {
  if (!phone?.trim()) return false;
  const digitsOnly = phone.replace(/\D/g, '');
  return digitsOnly.length >= 10 && digitsOnly.length <= 12;
};

export const isValidPassword = (password) => {
  if (!password) return false;
  return password.length >= 6;
};

export const isRequired = (value) => {
    return value?.trim() !== '';
};