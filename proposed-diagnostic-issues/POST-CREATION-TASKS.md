# Post-Issue Creation Tasks

After the three diagnostic issues are created and the documentation files are written, the following updates will be needed:

## 1. Update diagnostics overview

File: `/docs/diagnostics/overview.md`

Add these three new entries to the diagnostics table (in alphabetical order):

```markdown
| [`ASPIREPIPELINES001`](aspirepipelines001.md) | (Experimental) Error | <span id="ASPIREPIPELINES001"></span> Pipeline infrastructure APIs are experimental and subject to change. |
| [`ASPIREPIPELINES002`](aspirepipelines002.md) | (Experimental) Error | <span id="ASPIREPIPELINES002"></span> Deployment state management APIs are experimental and subject to change. |
| [`ASPIREPIPELINES003`](aspirepipelines003.md) | (Experimental) Error | <span id="ASPIREPIPELINES003"></span> Container image building APIs are experimental and subject to change. |
```

Insert these entries after the `ASPIREPUBLISHERS001` entry (around line 28-29).

## 2. Consider ASPIREPUBLISHERS001 Status

The existing `/docs/diagnostics/aspirepublishers001.md` file documents the old `ASPIREPUBLISHERS001` diagnostic code. Since PR #12416 splits this into three new codes, consider:

1. **Option A: Mark as obsolete**
   - Add a note at the top of `aspirepublishers001.md` indicating it has been split into three new diagnostics
   - Add links to the new diagnostic pages
   - Keep the file for historical reference

2. **Option B: Redirect**
   - Add a redirect entry in `.openpublishing.redirection.json` from the old page to a new overview page
   - Create a new page explaining the split

3. **Option C: Update in place**
   - Update the `aspirepublishers001.md` file to document all three new diagnostics
   - Add a note about the diagnostic code split

**Recommended:** Option A - mark as obsolete with clear guidance to the new diagnostics.

## 3. Example Obsolete Notice for ASPIREPUBLISHERS001

Add this notice at the top of `/docs/diagnostics/aspirepublishers001.md` after the YAML frontmatter:

```markdown
> [!IMPORTANT]
> As of Aspire 9.2, the `ASPIREPUBLISHERS001` diagnostic has been split into three more specific diagnostics:
> - [`ASPIREPIPELINES001`](aspirepipelines001.md) - Pipeline infrastructure APIs
> - [`ASPIREPIPELINES002`](aspirepipelines002.md) - Deployment state management APIs
> - [`ASPIREPIPELINES003`](aspirepipelines003.md) - Container image building APIs
>
> This allows developers to selectively suppress diagnostics based on which specific experimental features they're using.
```

## 4. File Naming Convention

When creating the documentation files, follow the existing naming pattern:
- Use lowercase for the diagnostic code in filenames
- Examples:
  - `aspirepipelines001.md`
  - `aspirepipelines002.md`
  - `aspirepipelines003.md`

## 5. YAML Frontmatter Template

Each documentation file should include appropriate frontmatter:

```yaml
---
title: Compiler Error ASPIREPIPELINESXXX
description: Learn more about compiler Error ASPIREPIPELINESXXX. [Brief description of what the diagnostic indicates].
ms.date: [Current date in MM/DD/YYYY format]
f1_keywords:
  - "ASPIREPIPELINESXXX"
helpviewer_keywords:
  - "ASPIREPIPELINESXXX"
---
```

## 6. Cross-references

Consider adding cross-references between the three new diagnostic pages since they're related:

- In ASPIREPIPELINES001: Mention the related diagnostics for state management and container building
- In ASPIREPIPELINES002: Mention the pipeline infrastructure diagnostic
- In ASPIREPIPELINES003: Mention the pipeline infrastructure diagnostic

Example:

```markdown
## Related diagnostics

- [`ASPIREPIPELINES002`](aspirepipelines002.md) - Deployment state management
- [`ASPIREPIPELINES003`](aspirepipelines003.md) - Container image building
```

## 7. Build and Test

After creating the documentation files:

1. Build the documentation locally to verify there are no errors
2. Check that all internal links work correctly
3. Verify code examples are properly formatted
4. Ensure markdown lint checks pass

## 8. PR Review Checklist

When submitting the documentation PR:

- [ ] All three diagnostic documentation files created
- [ ] Overview.md updated with new entries
- [ ] ASPIREPUBLISHERS001.md updated with obsolete notice
- [ ] All code examples tested for accuracy
- [ ] All markdown lint checks pass
- [ ] Build succeeds without warnings
- [ ] All internal links verified
- [ ] Appropriate ms.date values set
- [ ] Follow Microsoft Writing Style Guide conventions
