Constraint Satisfaction Problems Resolver Library
---
![.NET Core](https://github.com/sandhaka/csp/workflows/.NET%20Core/badge.svg)

#### *Project Status*
Active development

### Overview
Abstractions to use Constraint Satisfaction Problems and resolvers in your projects.

### How to use 
.1 Use directly the generic Csp\<T\> class, adding contraints, define domains and relationships through the Factory.

.2 Subclassing Csp\<T\> class to create your own implementation with custom members and methods overloading.

### Examples
See tests projects:
- MapColoringCsp: Color Australia regions with different colors in order to avoid conflicts (Every neighbor regions must has a different color).
- SchoolCalendar: Organize 1 week teaching plan for 3 different classes and 6 teachers.
- Sudoku: Solve a 9x9 Suduoku Grid