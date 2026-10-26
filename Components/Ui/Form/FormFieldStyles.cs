public static class FormFieldStyles
{
    public static string Control(bool invalid, bool textarea = false)
    {
        var resize = textarea ? "resize-y " : string.Empty;

        var border = invalid
            ? "border-red-500 focus:border-red-500 focus:ring-2 focus:ring-red-500/20"
            : "border-zinc-300 focus:border-zinc-900 focus:ring-2 focus:ring-zinc-900/10 dark:border-zinc-700 dark:focus:border-zinc-100";

        return resize +
               "w-full rounded-md border bg-white px-3 py-2 text-sm text-zinc-900 shadow-sm outline-none transition " +
               "placeholder:text-zinc-400 disabled:cursor-not-allowed disabled:bg-zinc-50 disabled:text-zinc-500 " +
               "dark:bg-zinc-950 dark:text-zinc-100 dark:placeholder:text-zinc-500 dark:disabled:bg-zinc-900 " +
               border;
    }
}