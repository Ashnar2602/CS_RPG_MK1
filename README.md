# CS_RPG_MK1

Un gioco di ruolo testuale multipiattaforma basato su SRD 5.1 con un Game Master controllato da intelligenza artificiale.

![Logo del progetto](https://via.placeholder.com/800x200?text=CS_RPG_MK1)

## Obiettivo

CS_RPG_MK1 è un'applicazione che mira a creare un'esperienza di gioco di ruolo immersiva e accessibile, utilizzando l'intelligenza artificiale per gestire il ruolo del Dungeon Master. Il gioco si basa sulle regole SRD 5.1 (System Reference Document) di D&D, rendendo l'esperienza familiare per i giocatori di giochi di ruolo tradizionali.

L'obiettivo principale è quello di permettere anche ai giocatori solitari di godersi un'avventura di ruolo completa, con descrizioni ricche, interazioni dinamiche e un sistema di combattimento tattico. L'AI agisce come un DM esperto, creando ambientazioni dettagliate, gestendo personaggi non giocanti e adattando l'avventura in base alle scelte del giocatore.

## Caratteristiche

- **DM controllato da AI**: Un Dungeon Master automatizzato che utilizza modelli linguistici avanzati per creare e gestire le avventure
- **Interfaccia intuitiva**: Interfaccia utente semplice e chiara per interagire con il mondo di gioco
- **Multipiattaforma**: Supporto per Windows, macOS e Linux
- **Sistema basato su SRD 5.1**: Implementazione delle meccaniche di gioco del famoso sistema d20
- **Salvataggio partite**: Possibilità di salvare e riprendere le avventure
- **Generazione procedurale**: Creazione di dungeon, ambientazioni e missioni procedurali

## Tecnologie utilizzate

- **C#**: Linguaggio di programmazione principale
- **Avalonia UI**: Framework multipiattaforma per l'interfaccia utente
- **OpenRouter**: Gateway API per l'accesso a modelli linguistici avanzati come:
  - deepseek-chat-v3
  - altri modelli supportati da OpenRouter
- **.NET 8**: Framework di sviluppo principale

## Prerequisiti

- .NET SDK 8.0 o superiore
- Una chiave API valida per OpenRouter

## Installazione

### Versione precompilata

1. Scarica l'ultima versione per il tuo sistema operativo dalla sezione [Releases](https://github.com/username/CS_RPG_MK1/releases)
2. Estrai il file zip in una cartella a tua scelta
3. Esegui l'applicazione CS_RPG_MK1

### Compilazione da sorgente

```bash
# Clona il repository
git clone https://github.com/username/CS_RPG_MK1.git

# Naviga nella directory
cd CS_RPG_MK1

# Compila il progetto
dotnet build

# Esegui l'applicazione
dotnet run
```

## Configurazione

Al primo avvio, ti verrà richiesto di inserire una chiave API di OpenRouter. Questa chiave è necessaria per il funzionamento del DM basato su AI. Per ottenere una chiave:

1. Registrati su [OpenRouter](https://openrouter.ai/)
2. Crea una nuova chiave API
3. Inserisci la chiave nella finestra di configurazione dell'applicazione

## Come utilizzare

1. Avvia l'applicazione
2. Configura la tua chiave API se richiesto
3. Nella casella di testo in basso, inserisci i comandi o le azioni che vuoi che il tuo personaggio esegua
4. L'AI risponderà nella finestra principale, descrivendo il risultato delle tue azioni e l'evoluzione dell'avventura

### Comandi di base

- `guarda`: Esamina l'ambiente circostante
- `parla a [NPC]`: Inizia una conversazione con un personaggio
- `raccogli [oggetto]`: Prendi un oggetto
- `attacca [bersaglio]`: Inizia un combattimento

## Licenza

Questo progetto è rilasciato sotto licenza MIT. Vedi il file [LICENSE](LICENSE) per maggiori dettagli.

Il contenuto basato su SRD 5.1 è utilizzato in conformità con la Open Game License (OGL) 1.0a.

## Contributi

I contributi sono benvenuti! Se desideri partecipare allo sviluppo, segui questi passaggi:

1. Fai un fork del repository
2. Crea un branch per la tua funzionalità (`git checkout -b feature/AmazingFeature`)
3. Committa le tue modifiche (`git commit -m 'Aggiunta una funzionalità incredibile'`)
4. Pusha il branch (`git push origin feature/AmazingFeature`)
5. Apri una Pull Request

## Changelog

### v0.1.0 (15 aprile 2025)
- Implementazione dell'interfaccia utente di base con Avalonia
- Integrazione con l'API di OpenRouter
- Finestra di configurazione per la chiave API
- Supporto per Windows, macOS e Linux
- Sistema di chat basilare con l'AI

### Roadmap
- [ ] Implementazione delle meccaniche di personaggio (caratteristiche, classi, razze)
- [ ] Sistema di combattimento
- [ ] Gestione dell'inventario
- [ ] Generazione procedurale di dungeon
- [ ] Sistema di salvataggio e caricamento delle partite
- [ ] Miglioramento del contesto dell'AI per maggiore coerenza narrativa

## Contatti

Nome del Progetto - [@twitter_handle](https://twitter.com/twitter_handle)

Link al Progetto: [https://github.com/username/CS_RPG_MK1](https://github.com/username/CS_RPG_MK1)