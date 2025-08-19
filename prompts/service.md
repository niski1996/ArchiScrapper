Jesteś ekspertem w dokumentowaniu aplikacji Event-Driven z modułami, projektami i integracjami. Twoim zadaniem jest wygenerować **nadrzędną dokumentację całego serwisu** na podstawie danych znajdujących się w folderze:

Copilot/Landscape/<nazwa_serwisu>/

Twoja dokumentacja ma mieć dwie wersje:

1. **Dokumentacja dla ludzi (Architecture_Human.md)**:
   - Opisuje nadrzędną architekturę serwisu, strukturę modułów i projektów.
   - Pokazuje zależności między modułami, integracje z API, bazami danych, joby, eventy globalne i inne procesy asynchroniczne.
   - Nie wchodzi w szczegóły implementacji (tych szukamy w folderach modułów), ale odsyła do odpowiednich folderów lub plików szczegółowej dokumentacji (`metadata.json` lub inne README w module).
   - Zawiera pełne zdania, narrację w języku naturalnym, spójną terminologię.
   - Generuje diagramy w Mervate ilustrujące:
     - zależności między modułami
     - przepływ eventów
     - powiązania z jobami, integracjami i bazami danych
   - Dla każdego modułu lub projektu podaje:
     - nazwę modułu/projektu
     - nadrzędną funkcję w serwisie
     - krótkie odwołania do integracji i eventów globalnych
     - linki lub ścieżki do szczegółowej dokumentacji modułu, jeśli ktoś chce zgłębić szczegóły

2. **Dokumentacja dla Copilota / LLM (Architecture_Copilot.md)**:
   - Zawiera tylko **kluczowe informacje techniczne** dotyczące całego serwisu:
     - lista wszystkich modułów/projektów w serwisie
     - zależności między modułami
     - globalne eventy i integracje
     - joby i bazy danych istotne na poziomie całego serwisu
   - Nie powiela szczegółów znajdujących się już w folderach modułów.
   - Jeśli potrzebne są szczegóły, odsyła do odpowiednich folderów lub plików `metadata.json`.
   - Wszystko, co jest w tej dokumentacji, musi być zgodne z wersją dla ludzi.
   - Dokumentacja ma być gęsta, łatwa do parsowania przez LLM i możliwa do użycia w promptach.

**Instrukcje działania**:
- Przejdź rekurencyjnie po folderze: `Copilot/Landscape/<nazwa_serwisu>/`
- Zidentyfikuj wszystkie projekty i moduły w serwisie.
- Zbierz dane globalne serwisu: zależności, eventy, integracje, joby, bazy danych.
- Stwórz hierarchiczną mapę serwisu z odwołaniami do szczegółowych folderów modułów.
- Wygeneruj dwa pliki:
  1. `Architecture_Human.md` – narracyjna, wizualna dokumentacja dla ludzi.
  2. `Architecture_Copilot.md` – techniczna, skondensowana dokumentacja dla Copilota.

**Dodatkowe wymagania**:
- Wersja dla ludzi może być dłuższa, z diagramami Mervate i opisami kontekstowymi.
- Wersja dla Copilota nie powinna powielać informacji szczegółowych modułów, ale odsyłać do nich w razie potrzeby.
- Obie dokumentacje muszą być spójne w zakresie faktów i nazw modułów/projektów.
- Zachowuj hierarchię serwisu w obu dokumentacjach, z zachowaniem zgodności między nimi.
