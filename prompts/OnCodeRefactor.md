# OnCodeRefactor

## Cel

Ten plik określa, jak Copilot powinien przeprowadzać refaktoryzację kodu w całym repozytorium mikroserwisów, zarówno na poziomie pojedynczych projektów/modułów, jak i na poziomie architektury serwisu.

Refaktoryzacja ma na celu:

1. Identyfikowanie powtarzalnych fragmentów kodu w mikroserwisach.
2. Identyfikowanie powszechnych wzorców projektowych, które mogą być użyte w innych serwisach.
3. Identyfikowanie klas, metod i struktur łamiących zasady SOLID.
4. Przygotowanie rekomendacji do poprawy czytelności, modularności i utrzymania kodu.

---

## 1. Przegląd repozytorium

* Copilot przechodzi przez repozytorium projekt po projekcie, moduł po module.
* Na poziomie mikroserwisu skanuje wszystkie pliki źródłowe.
* Na poziomie wyżej, w folderze architektury serwisu, konsoliduje informacje o powtarzalnych wzorcach i klasach/fragmentach kodu, które mogą być użyte w innych mikroserwisach.

---

## 2. Identyfikowanie powtarzalnych elementów

* Na poziomie mikroserwisu:

  * Szukaj fragmentów kodu, które powtarzają się **trzy razy lub więcej**.
  * Zidentyfikuj metody, funkcje lub klasy, które mogą zostać wyciągnięte do klasy bazowej lub abstrakcyjnej.
  * Zapisz je w folderze mikroserwisu jako potencjalne kandydatury do refaktoryzacji.

* Na poziomie architektury mikroserwisu:

  * Konsoliduj informacje o wzorcach, które pojawiają się w wielu serwisach.
  * Przechowuj lokalizacje, w których występują powtarzalne fragmenty.
  * Oceniaj potencjał do wyciągnięcia do narzędzi (tools) lub wspólnych bibliotek.

---

## 3. Wzorce projektowe

* Podczas skanowania identyfikuj powszechnie znane wzorce projektowe.
* Jeśli wzorzec powtarza się w jednym mikroserwisie, zapisuj go lokalnie.
* Jeśli wzorzec występuje w kilku mikroserwisach, zapisuj go w folderze architektury na wyższym poziomie, wraz z lokalizacjami jego występowania.

---

## 4. Identyfikacja naruszeń SOLID

* Na poziomie mikroserwisu analizuj klasy i metody pod kątem zasad SOLID:

  * Single Responsibility
  * Open/Closed
  * Liskov Substitution
  * Interface Segregation
  * Dependency Inversion
* Zapisuj klasy/metody, które łamią te zasady i rekomenduj sposoby ich uproszczenia lub refaktoryzacji.

---

## 5. Konsolidacja i rekomendacje

* Po przejściu przez wszystkie moduły mikroserwisu:

  * Konsoliduj listę powtarzalnych fragmentów kodu.
  * Konsoliduj listę powszechnych wzorców, które mogą zostać wykorzystane w innych mikroserwisach.
  * Twórz rekomendacje dotyczące:

    * Refaktoryzacji klas/metod łamiących SOLID
    * Wyciągnięcia powtarzalnych fragmentów do wspólnych narzędzi
    * Poprawy czytelności i modularności kodu
* Raport generowany w tym folderze architektury powinien zawierać lokalizacje wszystkich wykrytych problemów i propozycje zmian.

---

## 6. Priorytety Copilota

1. Najpierw identyfikacja powtarzalnych fragmentów i naruszeń SOLID w obrębie mikroserwisu.
2. Następnie analiza wzorców powtarzających się w wielu mikroserwisach.
3. Tworzenie rekomendacji do wyciągnięcia wspólnych narzędzi i poprawy czytelności kodu.
4. Wszystkie sugestie refaktoryzacji muszą być przeglądane i akceptowane przez człowieka przed wprowadzeniem zmian.

---

## Efekt

* Pełna mapa powtarzalnych fragmentów kodu w mikroserwisach.
* Zidentyfikowane naruszenia SOLID i rekomendacje ich naprawy.
* Wykryte powszechne wzorce projektowe, gotowe do ekstrakcji do wspólnych narzędzi.
* Raport jest gotowy do dalszej oceny i wdrożenia przez zespół deweloperski.
