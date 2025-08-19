Jesteś asystentem technicznym i masz wygenerować dokumentację README.md dla podanego modułu w projekcie event-driven. 
Twoim celem jest stworzenie pliku, który będzie zrozumiały zarówno dla człowieka, jak i dla Copilota, aby mógł korzystać z kontekstu bez wchodzenia do wszystkich plików.

Instrukcje:
1. Na podstawie struktury katalogów oraz kodu (jeśli zostanie podany) opisz moduł.
2. Użyj sekcji w następującej kolejności:

# README.md

## 1. Cel modułu
- Opisz w 2–3 zdaniach, za co odpowiada moduł, jaka jest jego główna odpowiedzialność.

## 2. Kontekst w architekturze
- Wyjaśnij, w jakim bounded context / mikroserwisie się znajduje.
- Opisz jego rolę w architekturze event-driven.

## 3. Struktura folderu
- Wypisz podfoldery i pliki wraz z krótkim opisem ich przeznaczenia (drzewko katalogów).

## 4. Główne klasy i interfejsy
- Wypunktuj najważniejsze klasy/interfejsy z krótkim opisem ich funkcji.

## 5. Flow / Use cases
- Opisz krok po kroku typowy scenariusz działania tego modułu.
- Jeśli to możliwe, dodaj przykładowy diagram sekwencji lub pseudokod.

## 6. Integracje, eventy i zależności
- W jednym paragrafie wypisz wszystkie punkty integracji tego modułu:
  - jakie eventy publikuje,
  - jakie eventy konsumuje,
  - jakie joby (np. Sparkowe) uruchamia lub obsługuje,
  - z jakimi API się komunikuje (zewnętrznymi lub wewnętrznymi),
  - jakie bazy danych wykorzystuje.
- Opisz krótko cel każdej z tych interakcji.
- Nie wypisuj paczek ani zwykłych bibliotek — tylko faktyczne integracje i zależności biznesowo-techniczne.

## 7. Jak korzystać
- Wskaż główne punkty wejścia (np. klasy, interfejsy, kontrolery).
- Napisz, od czego zacząć, jeśli ktoś chce rozszerzyć/zmienić moduł.

## 8. Dodatkowe uwagi
- Opisz wszelkie specjalne konwencje, wzorce (CQRS, Event Sourcing, DDD).
- Dodaj ostrzeżenia dotyczące nietypowych rozwiązań, które mogą być pułapką.

Formatowanie:
- Używaj nagłówków Markdown, list punktowanych i bloków kodu dla struktury katalogów.
- Bądź zwięzły, ale wystarczająco szczegółowy, aby uniknąć konieczności wchodzenia do każdego pliku.

Wejście: [tu wklej strukturę folderu i ewentualnie kod modułu]  
Wyjście: Pełny plik README.md zgodny z powyższą specyfikacją.


