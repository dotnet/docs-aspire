# Creating Diagnostic Issues

This directory contains structured information for creating three new GitHub issues to document diagnostic codes introduced in [PR #12416](https://github.com/dotnet/aspire/pull/12416).

## Background

PR #12416 split the single `ASPIREPUBLISHERS001` diagnostic into three more specific diagnostics:

1. **ASPIREPIPELINES001** - Pipeline infrastructure APIs
2. **ASPIREPIPELINES002** - Deployment state management APIs
3. **ASPIREPIPELINES003** - Container image building APIs

This split allows developers to selectively suppress diagnostics based on which specific experimental features they're using.

## Files in this Directory

- `ASPIREPIPELINES001.md` - Issue data for pipeline infrastructure diagnostic
- `ASPIREPIPELINES002.md` - Issue data for deployment state management diagnostic
- `ASPIREPIPELINES003.md` - Issue data for container image building diagnostic

## How to Create the Issues

Each markdown file contains all the information needed to create a GitHub issue using the diagnostic addition template.

### Option 1: Manual Issue Creation (Recommended)

For each diagnostic file:

1. Open the issue template: https://github.com/dotnet/docs-aspire/issues/new?template=06-diagnostic-addition.yml
2. Fill in the form fields using the values from the corresponding markdown file:
   - **Rule Type**: Copy from "Rule Type" section
   - **Version Introduced**: Copy from "Version Introduced" section
   - **Diagnostic ID**: Copy from "Diagnostic ID" section
   - **Short Title**: Copy from "Short Title" section
   - **Description**: Copy from "Description" section (including code examples)
   - **How to fix**: Copy from "How to fix" section (including code examples)
3. Submit the issue

### Option 2: Using GitHub CLI (If authenticated)

If you have GitHub CLI installed and authenticated:

```bash
# For ASPIREPIPELINES001
gh issue create \
  --repo dotnet/docs-aspire \
  --title "[Add diagnostics]: ASPIREPIPELINES001" \
  --label "Pri1,doc-idea,area-docs" \
  --assignee adegeo \
  --body "$(cat ASPIREPIPELINES001.md)"

# For ASPIREPIPELINES002
gh issue create \
  --repo dotnet/docs-aspire \
  --title "[Add diagnostics]: ASPIREPIPELINES002" \
  --label "Pri1,doc-idea,area-docs" \
  --assignee adegeo \
  --body "$(cat ASPIREPIPELINES002.md)"

# For ASPIREPIPELINES003
gh issue create \
  --repo dotnet/docs-aspire \
  --title "[Add diagnostics]: ASPIREPIPELINES003" \
  --label "Pri1,doc-idea,area-docs" \
  --assignee adegeo \
  --body "$(cat ASPIREPIPELINES003.md)"
```

**Note**: The issue template is a form-based template (YAML), so using `gh issue create` with `--body` might not work perfectly. Manual creation through the web interface is recommended.

### Option 3: Using GitHub API

If you're automating this with the GitHub API:

```bash
# Example for ASPIREPIPELINES001
curl -X POST \
  -H "Accept: application/vnd.github+json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  https://api.github.com/repos/dotnet/docs-aspire/issues \
  -d '{
    "title": "[Add diagnostics]: ASPIREPIPELINES001",
    "body": "...", # Content from ASPIREPIPELINES001.md
    "labels": ["Pri1", "doc-idea", "area-docs"],
    "assignees": ["adegeo"]
  }'
```

## Expected Outcome

After creating these three issues:

1. The documentation team will be notified (via assignment to @adegeo)
2. Each issue will be tracked with `Pri1`, `doc-idea`, and `area-docs` labels
3. Documentation pages will be created in `/docs/diagnostics/` directory following the pattern of existing diagnostics like `aspirepublishers001.md`
4. The new diagnostic documentation will include:
   - Overview of what triggers the diagnostic
   - List of affected APIs
   - Code examples showing the diagnostic
   - Instructions for suppressing the diagnostic

## Related Files

After the issues are created and documentation is written, the following files will likely be created:

- `/docs/diagnostics/aspirepipelines001.md` - Documentation for ASPIREPIPELINES001
- `/docs/diagnostics/aspirepipelines002.md` - Documentation for ASPIREPIPELINES002  
- `/docs/diagnostics/aspirepipelines003.md` - Documentation for ASPIREPIPELINES003
- Updates to `/docs/diagnostics/overview.md` to include the new diagnostics

The existing `/docs/diagnostics/aspirepublishers001.md` may need to be updated or marked as obsolete since these three new diagnostics replace it.
