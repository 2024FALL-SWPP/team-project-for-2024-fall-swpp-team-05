# PR Title [Descriptive title summarizing the change]
## Related Issue(s)
Link or reference any related issues or tickets.
## PR Description
[Provide a brief summary of the changes made.]
### Changes Included
- [ ] Added new feature(s)
- [ ] Fixed identified bug(s)
- [ ] Updated relevant documentation
### Screenshots (if UI changes were made)
Attach screenshots or GIFs of any visual changes. (Only for frontend-related changes)
### Notes for Reviewer
Any specific instructions or points to be considered by the reviewer.
---
## Reviewer Checklist
- [ ] Code is written in clean, maintainable, and idiomatic form.
- [ ] Automated test coverage is adequate.
- [ ] All existing tests pass.
- [ ] Manual testing has been performed to ensure the PR works as expected.
- [ ] Code review comments have been addressed or clarified.
---
## Additional Comments
Add any other comments or information that might be useful for the review process.
linter.yml:
---
name: Lint
on: # yamllint disable-line rule:truthy
  push: null
  pull_request: null
permissions: {}
jobs:
  build:
    name: Lint
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: read
      # To report GitHub Actions status checks
      statuses: write
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
        with:
          # super-linter needs the full git history to get the
          # list of files that changed across commits
          fetch-depth: 0
      - name: Super-linter
        uses: super-linter/super-linter@v7.1.0 # x-release-please-version
        env:
          # To report GitHub Actions status checks
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
