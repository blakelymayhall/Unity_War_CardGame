- For getting started, we're going to make feature branches off slightly longer lived 'phase' branches (see [planning](planning.md))
- Branch names will follow the following criteria:
	- Phase branches: \<JiraTag>_\<shortname>
	- Feature branches: \<JiraTag>_\<shortname>

```mermaid
%%{ init: { 'theme': 'base', 'themeVariables': { 'primaryColor': '#BB2528', 'primaryTextColor': '#fff', 'primaryBorderColor': '#7C0000', 'lineColor': '#F8B229', 'secondaryColor': '#006100', 'tertiaryColor': '#fff' } } }%%
gitGraph
   commit
   commit
   branch uwc-1_phase1
   checkout uwc-1_phase1
   branch uwc-3_card
   checkout uwc-3_card
   commit
   commit
   checkout uwc-1_phase1
   merge uwc-3_card
   branch uwc-4_cardclick
   checkout uwc-4_cardclick
   commit
   commit
   checkout uwc-1_phase1
   merge uwc-4_cardclick
   checkout uwc-3_card
   checkout main
   merge uwc-1_phase1
   commit
   commit
```
