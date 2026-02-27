# UML Delta Report — Refactor: Static → DI & Interfaces

## 🏗 Architectural Impact

Цей документ показує, як змінилася архітектура системи після **Dependency Injection та інтерфейсів**, з акцентом на UML-елементи.

---

## 1️⃣ Class & Component Diagrams (Static View)

**Що змінилося:**

- Статичний клас `LinkRepository` і статичні виклики замінено на **екземплярний клас**, який реалізує `ILinkRepository`.
- `CitationService.GenerateCitation` став методом екземпляру, реалізує `ICitationService`.
- Форми (`MainForm`, `AddEditForm`, `CategoryManagerForm`, `ExportForm`) тепер залежать від **інтерфейсів**, а не від конкретних класів.
- З’явився шар абстракції між UI і репозиторієм/сервісами.

**Елементи, додані/зміненi:**

| Елемент | Before | After |
|---------|--------|-------|
| LinkRepository | `static class` | implements `ILinkRepository`, non-static |
| CitationService | static method `GenerateCitation` | implements `ICitationService` |
| Forms | залежність від конкретних класів | конструктор приймає інтерфейси (`ILinkRepository`, `ICitationService`) |
| Interfaces | — | `ILinkRepository`, `ICitationService` |

**Практичний ефект:**

- Чіткіші залежності, інверсія керування (IoC)
- Можливість легко мокати сервіси для юніт-тестів
- Стрілки залежностей на UML діаграмах тепер йдуть до інтерфейсів, а не до реалізацій

---

## 2️⃣ Sequence & Activity Diagrams (Dynamic View)

**Що змінилося:**

- Виклики до статичних методів замінено на **через екземпляри сервісів** (`_repoInstance.Method()`).
- `Program.cs` тепер **Composition Root**: створює `ServiceCollection`, реєструє сервіси і запускає `MainForm` через DI.

**Flow (Execution) зміни:**

| Flow | Before | After |
|------|--------|-------|
| Add | `LinkRepository.Add(link)` (статичний) | `_repo.Add(link)` (через інтерфейс) |
| Edit | прямий виклик репозиторію | `_repo.Update(link)` через інтерфейс |
| Export | `CitationService.GenerateList(links, style)` (статичний) | `_citationService.GenerateList(_repo.GetAll(), style)` |

**Практичний ефект:**

- Потік активностей незмінний, але **орchestration** централізовано через інтерфейси
- Кожна форма працює з екземпляром сервісу, а не з глобальною статичною пам’яттю

---

## 3️⃣ Use Case Diagram

**Зміни:**

- User-level use cases залишилися без змін: Add/Edit/Delete/Manage Categories/Export References
- Внутрішні виклики делегуються до **сервісних інтерфейсів**
- Нові внутрішні “ролі” (не актори): `ILinkRepository`, `ICitationService`

**Практичний ефект:**

- UI use cases делегують роботу до портів (інтерфейсів)
- Краща тестованість та замінність backend компонентів

---

## 4️⃣ Deployment & Object Diagrams (Runtime View)

**Зміни:**

- Статичний глобальний клас замінено на **екземпляр сервісу**.
- Появився DI контейнер (`ServiceProvider`) як “гравець” у діаграмі об’єктів.

**Об’єкти під час виконання:**

| Об’єкт | Before | After |
|---------|--------|-------|
| LinkRepository | глобальний статичний | singleton через DI, передається у форми |
| CitationService | статичний | singleton/екземпляр через DI |
| Forms | зверталися до статичних класів | приймають `_repo` та `_citationService` через конструктор |
| Program | простий запуск MainForm | створює ServiceCollection та DI Container |

---

## 5️⃣ State Diagram

**Зміни:**

- Стани форм залишилися, але всі дії відбуваються через **екземпляри сервісів**:
  - `MainForm Idle -> Add/Edit Form -> Saved -> MainForm Idle` (через `_repo.Add`/`Update`)
  - Export Flow використовує `_citation_service.GenerateList(...)`

**Практичний ефект:**

- Сторони ефекти (persist/export) тепер виконуються через замінні сервіси
- Діаграма станів не змінилася структурно, але виклики замінені на інтерфейсні

---

## 6️⃣ Summary Table — Key Changes

| Aspect | Before | After | Effect |
|--------|--------|-------|--------|
| LinkRepository | static | instance + `ILinkRepository` | IoC, singleton, тестування |
| CitationService | static | instance + `ICitationService` | DI, централізована логіка |
| Forms | залежність від класів | залежність від інтерфейсів | легше тестувати |
| Program | простий запуск MainForm | Composition Root + DI | керує життям об’єктів |
| Calls | LinkRepository.Method() | _repo.Method() | виклики через інтерфейси |
| Runtime | глобальні дані | екземплярні сервіси | прозорий стан, DI container |
| Use Cases | UI -> static calls | UI -> interface calls | замінність бекенду |
| Activity Flow | прямий | через DI сервіси | краща оркестрація |

---

Цей Markdown дозволяє **швидко порівняти UML до і після рефакторингу** без перемальовування діаграм.