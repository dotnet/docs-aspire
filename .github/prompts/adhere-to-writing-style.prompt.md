The following is a set of guidelines to follow when writing documentation for Aspire. Please ensure that all documentation adheres to these guidelines:

## Voice and tone

Write for readers who may have a limited vocabulary or are not native English speakers. Use simple, clear language and avoid jargon whenever possible. If you must use technical terms, provide a brief explanation of what they mean.

- Use active voice
- Use the second person (you, your) to address the reader directly

## Lists

Use lists more sparingly. Use them only when they help to clarify the content. Use bullet points for unordered lists and numbered lists for ordered lists. Regardless of the type of list, always use sentence-style capitalization, complete sentences, and proper punctuation.

## Headings

Headings use sentence-style capitalization. Always capitalize the first word of a heading. Do not use gerunds (e.g., "Using" or "Creating") in heading.

## Text styling

Use _italics_ for files, folders, paths (for long items, split onto their own line), new terms.

Use **bold** for UI elements.

Use `code` for inline code, language keywords, NuGet package names, command-line commands, database table and column names, and URLs that you don't want to be clickable.

## Links

Strive to use relative links whenever possible. Use absolute links only when necessary. For example, if you are linking to a page on a different site, use an absolute link. When using absolute links, use the full URL (including the protocol) and remove the locale from the URL. Avoid HTTP links, always prefer HTTPS. Whenever providing additional resources, use the following format:

"For more information, see [link text](../relative/link/to/content.md)."

Never link to `https://learn.microsoft.com` or `https://learn.microsoft.com/en-us`, always remove these and instead use site relative links. For example, if the original link is `https://learn.microsoft.com/en-us/aspnet/core/mvc/overview`, it should be changed to `/aspnet/core/mvc/overview`.

## Things to avoid

- Avoid future tense: In some non-English languages the concept of future tense is not the same as English. Using future tense can make your documents harder to read.
- Avoid passive voice: Passive voice can make your writing less clear and harder to read. Use active voice whenever possible.
