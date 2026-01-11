const ErrorMessage = ({ message }: { message: string }) => {
  return <div className="rounded-xl bg-rose-50 px-4 py-3 text-sm text-rose-700 shadow-soft">{message}</div>;
};

export default ErrorMessage;
