# Code Rules

## Cel

Ten dokument określa zasady pisania czystego, czytelnego i utrzymywalnego kodu, bazujące przede wszystkim na wytycznych zawartych w książce Roberta C. Martina "Czysty kod". Copilot powinien traktować te zasady priorytetowo przy generowaniu lub refaktoryzacji kodu.

---

## 1. Nazwy i czytelność

* Nazwy klas, metod, zmiennych i funkcji muszą być czytelne i jednoznaczne.
* Nazwy powinny jasno określać intencję i funkcjonalność.
* Unikaj skrótów, które nie są powszechnie znane.
* Funkcje powinny mieć maksymalnie jedną odpowiedzialność.

---

## 2. Funkcje i metody

* Każda funkcja/metoda powinna wykonywać jedną rzecz (Single Responsibility).
* Funkcje powinny być krótkie, najlepiej nie dłuższe niż 20–25 linijek.
* Parametry funkcji należy ograniczać do minimum.
* Funkcje powinny mieć sensowną hierarchię wywołań – wyższy poziom opisuje "co" a niższy "jak".

---

## 3. Klasy i struktura obiektowa

* Klasy powinny mieć jedną odpowiedzialność (SRP).
* Unikać duplikowania logiki w wielu klasach.
* Implementować wzorce projektowe tam, gdzie zwiększają czytelność i modularność.
* Zachowywać zasady Open/Closed, Liskov, Interface Segregation, Dependency Inversion (SOLID).

---

## 4. Komentarze i dokumentacja

* Komentarze używamy tylko tam, gdzie kod nie jest jednoznaczny.
* Preferowane jest pisanie kodu, który sam się dokumentuje.
* Nie zostawiać komentarzy typu TODO lub FIXME bez kontekstu.

---

## 5. Struktura kodu

* Zachowuj spójny styl formatowania w całym repozytorium.
* Używaj modułów i pakietów do logicznego grupowania kodu.
* Pliki nie powinny być nadmiernie długie ani łączyć wielu odpowiedzialności.

---

## 6. Testy

* Pisanie testów jednostkowych i integracyjnych jest obowiązkowe.
* Testy powinny być czytelne, jednoznaczne i łatwe do utrzymania.
* Każda funkcjonalność powinna mieć pokrycie testami.

---

## 7. Refaktoryzacja

* Systematycznie identyfikuj duplikacje i powtarzalne fragmenty.
* Upraszczaj złożone funkcje i klasy.
* Zwracaj uwagę na przestrzeganie zasad SOLID przy refaktoryzacji.
* Każda zmiana powinna być przemyślana i weryfikowana przez człowieka.

---

## 8. Priorytety Copilota

1. Najpierw stosowanie zasad "Czystego kodu" Roberta C. Martina.
2. Następnie wzorce projektowe i dobre praktyki branżowe.
3. Dopiero na końcu zdrowy rozsądek i kontekst specyficzny projektu.

---

## Efekt

* Kod czytelny, łatwy w utrzymaniu i rozwoju.
* Jednolity styl w całym repozytorium.
* Minimalizacja duplikacji i naruszeń zasad SOLID.
* Testy zapewniające bezpieczeństwo zmian i łatwe weryfikacje funkcjonalności.
