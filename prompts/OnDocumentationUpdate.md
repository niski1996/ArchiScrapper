# OnDocumentationUpdate

## Cel

Ten plik określa zasady aktualizacji dokumentacji mikroserwisów w centralnym repozytorium. Dokumentacja składa się z dwóch warstw:

1. **H_** – pliki human-readable, pełny opis architektury i modułów dla ludzi  
2. **C_** – pliki Copilot-readable, zoptymalizowane pod kątem tokenów, zawierające jedynie niezbędne informacje do analizy systemu

Aktualizacje muszą zachować spójność między tymi dwoma warstwami.

---

## Zasady aktualizacji

### 1. Folder tymczasowy

- Istnieje **jeden wspólny folder na pliki tymczasowe** dla Copilota, w którym przechowywane są analizy i tymczasowe dane potrzebne do aktualizacji dokumentacji.  
- Podczas pracy nad danym modułem:
  - Copilot zapisuje w tym folderze wszystkie tymczasowe informacje (diffy, listy integracji, stany poprzednich commitów).  
- Po zakończeniu pracy nad modułem:
  - Folder tymczasowy jest nadpisywany przy pracy nad następnym module – nie tworzy się osobnych folderów dla modułów.

### 2. Identyfikacja zmian

- Zidentyfikuj **branch i commit**, na którym pracujesz.  
- Wykorzystaj **changelog** w plikach dokumentacji do wygenerowania **diffa** między aktualną wersją a ostatnią wersją.  
- Zmiany w dokumentacji należy wprowadzać **tylko tam, gdzie jesteś pewien**, że coś rzeczywiście się zmieniło lub pojawiło się nowe.

### 3. Zakres zmian

- Zmiany ograniczaj do:
  - nowych lub zmodyfikowanych modułów, klas lub funkcjonalności, które wpływają na integracje lub architekturę,
  - punktów wejścia/wyjścia modułów (eventy, API, bazy danych),
  - aktualizacji COP-plików dla Copilota.

- **Nie aktualizuj grafów UML ani diagramów Mermaid**, jeśli nie jesteś pewien zmian w relacjach między modułami lub serwisami.  
- Nie aktualizuj trywialnych zmian, które nie mają wpływu na architekturę (np. komentarze w kodzie bez zmiany API).

### 4. Konsultacja z człowiekiem

- Każda aktualizacja dokumentacji wymaga **weryfikacji przez człowieka** przed zatwierdzeniem:  
  - Copilot przedstawia proponowane zmiany i integracje.  
  - Człowiek ocenia, czy zmiany są sensowne i zgodne z rzeczywistym działaniem systemu.  
- Dopiero po zatwierdzeniu zmiany zostają scalone do centralnego repozytorium.

### 5. Spójność między plikami

- Plik **C_** musi być **w pełni zawarty w pliku H_**.  
- Plik **H_** może zawierać dodatkowe, opisowe informacje, które nie występują w C_ (np. wzorce projektowe, kontekst).  
- Wszystkie zmiany w plikach C_ muszą być odzwierciedlone w plikach H_.

### 6. Changelog

- Każdy plik dokumentacji posiada sekcję **Changelog**:  
  ```markdown
  ## Changelog
  - <hash commit>: <YYYY-MM-DD>: <opis zmiany>
