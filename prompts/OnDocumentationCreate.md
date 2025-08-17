# OnDocumentationCreate

## Cel pliku

Ten plik definiuje **zasady tworzenia i aktualizacji dokumentacji** w repozytorium centralnym dla mikroserwisów i ich modułów.
Dokumentacja jest utrzymywana w **dwóch równoległych warstwach**:

1. **Warstwa human-readable (H\_)** – pliki opisowe, przeznaczone dla ludzi.

   * Zawierają pełne, łatwe do zrozumienia wyjaśnienia działania mikroserwisu lub modułu.
   * Mogą zawierać informacje nadmiarowe, kontekstowe, przykłady i objaśnienia wzorców projektowych.
   * **Wizualizacja architektury**: wszystkie powiązania i struktury powinny być przedstawione **wyłącznie przy użyciu Mermaid UML** (diagramy klas, sekwencji, komponentów, przepływów itp.).

2. **Warstwa Copilot-readable (C\_)** – pliki zoptymalizowane pod Copilota.

   * Zawierają **tylko niezbędne, konkretne informacje** o tym, co Copilot nie może odczytać z ogólnie dostępnych źródeł.
   * Maksymalnie zwięzłe, minimalizując liczbę tokenów.

---

## Zasady ogólne

1. **Spójność**

   * Wszystkie informacje w pliku Copilota muszą być zawarte w odpowiednim pliku dla ludzi.
   * Plik dla ludzi może zawierać dodatkowe informacje, które nie znajdują się w pliku Copilota.
   * Nigdy nie powinno dochodzić do sytuacji, w której plik dla ludzi zawiera informacje sprzeczne z plikiem Copilota.

2. **Zakres dokumentacji**

   * **Moduł projektu**:

     * `H_<module-name>.md` – opis human-readable, cele modułu, odpowiedzialności, zależności, przykłady użycia, **diagramy UML w Mermaid**.
     * `C_<module-name>.yaml|json` – zoptymalizowany pod Copilot, tylko niezbędne fakty.
   * **Mikroserwis**:

     * `H_Architecture.md` – opis głównych celów mikroserwisu, przepływów komunikacji, integracji i założeń architektonicznych, **diagramy UML w Mermaid**.
     * `C_IO.yaml|json` – zwięzły opis wejść/wyjść mikroserwisu, zależności, integracji.

3. **Changelog**

   * Każdy plik dokumentacji musi zawierać **changelog**, w którym zapisany jest:

     * Hash ostatniego commita, w którym plik był aktualizowany
     * Data ostatniej aktualizacji
   * Przykład wpisu w Markdown:

```markdown
## Changelog
- Commit: 3a1f5b7
- Updated: 2025-08-16
```

---

## Generacja dokumentacji i integracje między serwisami

1. **Zakres integracji**

   Dla każdego mikroserwisu Copilot generuje **pełną listę integracji** z innymi serwisami:

   * Eventy przyjmowane: typ eventu, źródło, struktura danych
   * Eventy wysyłane: typ eventu, docelowy mikroserwis, struktura danych
   * API wykorzystywane: jakie API innych mikroserwisów są konsumowane
   * API wystawiane: które API są udostępniane innym serwisom
   * Bazy danych: z jakich baz mikroserwis pobiera dane, gdzie zapisuje lub wysyła wyniki
   * Inne punkty integracji: kolejki, systemy messagingowe, cache

   Wszystkie powyższe informacje są przechowywane **w jednym pliku Copilot-readable** (`C_IO.yaml|json`) dla danego mikroserwisu.

2. **Tworzenie diagramów UML**

   * Każdy mikroserwis i moduł powinien mieć **diagramy UML w Mermaid** pokazujące relacje, nawet jeśli nie wszystkie połączenia są obecnie znane.
   * Jeśli mikroserwisy lub moduły wydają się niepowiązane, należy umieścić je w diagramie osobno – diagram pokazuje, że połączenia są niepewne i wymagają weryfikacji w kolejnym skanie.

3. **Proces generowania dokumentacji**

   * **Krok 1 – wybór modułu**: Copilot analizuje wybrany moduł mikroserwisu.
   * **Krok 2 – generowanie dokumentacji modułu**: tworzone są dwa pliki:

     1. `H_<module-name>.md` – human-readable, z diagramami Mermaid
     2. `C_<module-name>.yaml|json` – Copilot-readable, zoptymalizowany pod tokeny
   * **Krok 3 – uzupełnianie dokumentacji mikroserwisu**: informacje o module dopisywane są do:

     1. `H_Architecture.md` – dla ludzi
     2. `C_IO.yaml|json` – dla Copilota
   * **Krok 4 – akceptacja człowieka**:

     * Copilot prezentuje zaktualizowane informacje dotyczące integracji i przepływów w mikroserwisie
     * Człowiek ocenia, czy wygenerowana dokumentacja jest sensowna i poprawna
     * Po zatwierdzeniu dokumentacja zostaje scalona do repozytorium centralnego

4. **Zasady spójności**

   * Informacje w pliku Copilota (`C_`) muszą być w pełni zawarte w pliku dla ludzi (`H_`).
   * W przypadku zmian w innych mikroserwisach, Copilot powinien wykrywać, które integracje uległy zmianie i zaproponować aktualizacje dokumentacji.
   * Człowiek zawsze zatwierdza zmiany dotyczące przepływów między mikroserwisami przed scaleniem ich do centralnej dokumentacji.

---

## Efekt

Dzięki tym zasadom:

* Każdy mikroserwis ma aktualny i kompletny opis integracji z innymi serwisami.
* Pliki H\_ zapewniają pełny kontekst dla ludzi i wizualizację architektury w Mermaid.
* Pliki C\_ zawierają niezbędne, konkretne informacje dla Copilota, zoptymalizowane pod tokeny.
* Architektura systemu jest spójna i możliwa do szybkiej analizy w przypadku zmian w poszczególnych mikroserwisach.
