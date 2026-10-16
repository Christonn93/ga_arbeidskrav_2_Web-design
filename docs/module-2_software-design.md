# Arbeidskrav 2 – Modul 2: Software Design

## Emne 3: Avansert Backend-programmering

Dette arbeidskravet er individuelt. Du skal designe og bygge en Blazor-applikasjon for brettspillkafeen Kafé Terningkast, og demonstrere, begrunne og reflektere over egne designvalg. Arbeidet skal vise at du kan utvikle funksjonelle, vedlikeholdbare og godt strukturerte løsninger innenfor temaene i modul 2.

| Felt               | Informasjon                                                                                      |
|--------------------|--------------------------------------------------------------------------------------------------|
| Emne               | Emne 3: Avansert Backend-programmering                                                           |
| Modul              | Modul 2: Software Design                                                                         |
| Vurderingsform     | Godkjent / Ikke godkjent                                                                         |
| Gjennomføring      | Høst 2026                                                                                        |
| Åpner              | Fredag 9. oktober 2026 kl. 00:00                                                                 |
| Innleveringsfrist  | Fredag 30. oktober 2026 kl. 23:59                                                                |
| Innlevering        | Leveres i Moodle.                                                                                |
| Innleveringsformat | Én ZIP-fil med kode, README.md, designdokumentasjon, KI-refleksjon og videofil eller videolenke. |

## Bruk av KI (kunstig intelligens)

Bruk av kunstig intelligens (KI) er tillatt i arbeidet med innleveringen. KI kan brukes som støtteverktøy, for eksempel til idéutvikling, forklaringer, feilsøking, strukturering, kodeforslag og språklig forbedring.

Du er selv ansvarlig for innholdet du leverer, og skal kunne forstå, forklare og begrunne løsningen din.

### Refleksjon over bruk av KI

Dersom du har brukt KI i arbeidet, skal du kort beskrive og reflektere over hvordan KI har påvirket besvarelsen. Du trenger ikke å dokumentere eller levere alle prompter og svar.

Refleksjonen skal kort beskrive:

- Hvordan KI ble brukt i arbeidet.
- Hva KI bidro med, for eksempel idéer, kode, forklaringer, feilsøking eller forbedringer.
- Hvordan KI påvirket den endelige løsningen.
- Hva du selv har vurdert, endret eller videreutviklet fra forslagene du fikk.
- Eventuelle feil, begrensninger eller utfordringer du oppdaget ved bruk av KI.

Refleksjonen skal være en del av innleveringen, enten i `README.md` eller i en egen fil.

Dersom du ikke har brukt KI, holder det å opplyse om dette.

### Viktig

KI er et hjelpemiddel og erstatter ikke kandidatens egen forståelse. Du skal kunne gjøre rede for hele besvarelsen, inkludert kode, valg av løsning og sentrale faglige vurderinger.

Ved tvil om kandidatens selvstendige forståelse av besvarelsen kan kandidaten bli bedt om å forklare eller demonstrere løsningen muntlig.

Bruk av KI skal følge skolens gjeldende retningslinjer for KI, kildebruk og akademisk integritet.

## Læringsutbytter som vurderes

Arbeidskravet vurderer følgende læringsutbytter. De tre første er hentet fra emnebeskrivelsen. De øvrige er læringsmål fra ukeplanen for modul 2.

### Fra emnebeskrivelsen

- har forståelse for prinsipper som bidrar til ren, vedlikeholdbar og effektiv kode i praksis
- kan skrive programkode som løser avanserte og komplekse problemstillinger
- har evne til å tenke over og løse avanserte og komplekse problemer

### Fra ukeplanen for modul 2

- kan bygge komponentbaserte grensesnitt i Blazor med Razor-syntaks, databinding og hendelser
- kan lage gjenbrukbare komponenter som kommuniserer med parametere og hendelser
- kan bruke routing, navigasjon og URL-parametere
- kan skille mellom lokal tilstand og applikasjonstilstand, og styre dataflyten i en applikasjon
- kan lage responsive grensesnitt
- kan bruke HttpClient til asynkrone kall og CRUD mot et REST API med JSON
- har kjennskap til komponentlivssyklus og gjengivelsesmodi
- kan modellere og dokumentere software design på en enkel måte
- kan vurdere designalternativer og begrunne egne designvalg faglig
- kan teste sentrale deler av egen løsning

| Hovedoppgave                                  | Læringsutbytter som særlig vurderes                                                          |
|-----------------------------------------------|----------------------------------------------------------------------------------------------|
| 1. Spillhylla                                 | Komponenter, Razor-syntaks, databinding, hendelser og komponentansvar                        |
| 2. Terningkast og spillsider                  | Gjenbrukbare komponenter, parametere, hendelser, routing og URL-parametere                   |
| 3. Min spillekveld                            | Lokal tilstand, applikasjonstilstand, dataflyt, responsivt grensesnitt og designalternativer |
| 4. Bak disken                                 | Asynkrone kall, JSON, CRUD mot REST API, livssyklus og gjengivelsesmodi                      |
| 5. Designdokumentasjon, testing og designvalg | Modellering, dokumentasjon, testing og faglig begrunnede designvalg                          |
| 6. Videopresentasjon og refleksjon            | Faglig begrunnelse, refleksjon og forståelse av egen løsning                                 |

## Caset: Kafé Terningkast

Kafé Terningkast er en brettspillkafé med fulle hyller og én papirliste som ingen oppdaterer. Gjestene bruker tjue minutter på å velge spill, og de ansatte vet ikke hvilke spill som faktisk står i hylla. Eieren har bedt deg bygge en nettapp som løser dette.

Gjestene skal kunne finne et spill som passer gruppen, gi spillene terningkast og planlegge en spillekveld. De ansatte skal kunne vedlikeholde spillsamlingen. Du bygger appen trinn for trinn gjennom oppgavene.

| Del av appen              | Hva brukeren gjør                                              | Oppgave |
|---------------------------|----------------------------------------------------------------|---------|
| Spillhylla                | Blar, søker og filtrerer i spillene, eller lar terningen velge | 1       |
| Terningkast og spillsider | Åpner et spill, leser detaljene og gir terningkast fra 1 til 6 | 2       |
| Min spillekveld           | Samler spill til kvelden og ser samlet spilletid               | 3       |
| Bak disken                | Ansatte legger til, endrer og sletter spill                    | 4       |

Spillsamlingen er din. Velg minst åtte spill, ekte eller oppdiktede. Modellen under er utgangspunktet. Du kan utvide den, men ikke fjerne feltene.

```csharp
public class BoardGame
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int PlayTimeMinutes { get; set; }
    public string Category { get; set; } = "";
    public int Rating { get; set; }          // dice roll, 1-6
    public string Description { get; set; } = "";
}
```

## Felles rammer for kode og dokumentasjon

Lag én ryddig .NET-løsning basert på malen Blazor Web App. Løsningen skal kunne bygges og kjøres uten andre manuelle steg enn dem `README.md` beskriver.

Bruk engelsk i C#-identifikatorer, komponentnavn, filnavn og ruter. Følg PascalCase for typer, komponenter, metoder og offentlige medlemmer, og camelCase for lokale variabler og metodeparametere. Komponentparametere merket med `[Parameter]` er offentlige egenskaper og skrives i PascalCase. Tekst i brukergrensesnittet kan være på norsk. Bruk beskrivende navn, og kommenter bare når kommentaren forklarer noe som ikke allerede fremgår av koden.

Hold ansvaret adskilt. Komponenter viser data og tar imot brukerhandlinger. Data, tilstand og API-kall ligger i egne klasser.

Komponentene oppgavene ber deg lage, skal du skrive selv. Ferdige komponentbiblioteker som MudBlazor eller Radzen skal ikke erstatte dem. Et CSS-rammeverk som Bootstrap er tillatt, men du må kunne forklare stilene du bruker.

Du kan bruke mønstrene fra undervisningen, men løsningen skal være din egen og bygget for dette caset.

`README.md` skal beskrive hvordan løsningen bygges og kjøres, sidene og komponentene i appen, designvalg, testtilfeller, resultater, begrensninger og refleksjon over KI-bruk.

## Oppgave 1 – Spillhylla: komponenter og databinding

### Formål

Bygg siden der gjestene finner spill. Du deler grensesnittet i komponenter med tydelig ansvar, og bruker databinding og hendelser til søk, filtrering og enkel interaktivitet.

### Krav

- Lag siden `/games` som viser alle spillene i samlingen. I denne oppgaven kan dataene ligge som en liste i en egen klasse. Listen skal ikke ligge i selve komponenten.
- Lag komponenten `GameCard` som viser ett spill. Komponenten får spillet inn som parameter og har ikke ansvar for listen.
- Vis listen med `@foreach`, og bruk `@if` til å vise ulikt innhold når listen har treff og når den er tom.
- Lag et søkefelt bundet med `@bind` som filtrerer på navn mens brukeren skriver. Søket skal ikke skille mellom store og små bokstaver.
- Lag et filter for antall spillere. Et spill vises når antallet ligger mellom `MinPlayers` og `MaxPlayers`.
- Vis antall treff, og oppdater tallet når søk eller filter endres.
- Lag knappen «Trill terningen» som velger et tilfeldig spill blant treffene og fremhever det. Bruk `@onclick`.
- Forklar i `README.md` hvilket ansvar siden og `GameCard` har, og hvorfor du har delt dem slik.

### Testtilfeller og kanttilfeller

- Søk etter et spill som finnes, med både små og store bokstaver.
- Søk etter noe som ikke finnes. Siden skal vise en tydelig melding, og «Trill terningen» skal ikke krasje når det ikke er noen treff.
- Test grenseverdiene i spillerfilteret. Et spill for 2–4 spillere skal vises for 2 og 4, men ikke for 1 eller 5.
- Kombiner søk og filter, og kontroller at antall treff stemmer.
- Tøm søkefeltet, og kontroller at hele samlingen vises igjen.

### Leveranse og vurderingspunkter

Lever siden, komponenten og testresultatene, og beskriv ansvarsfordelingen i `README.md`. Det vurderes om grensesnittet er delt i komponenter med tydelig ansvar, om databinding og hendelser brukes riktig, om kanttilfellene er håndtert, og om koden er lett å lese og endre.

> Tips: Samle filtreringen i én metode eller egenskap i C#. Da slipper du logikk spredt rundt i markupen, og den blir enkel å teste i oppgave 5.

Kobling til læringsutbytte: komponenter, Razor-syntaks, databinding, hendelser, komponentansvar og vedlikeholdbar kode.

## Oppgave 2 – Terningkast og spillsider: komponentkommunikasjon og navigasjon

### Formål

Lag en gjenbrukbar komponent som kommuniserer begge veier med forelderen, og gi hvert spill sin egen side. Du bruker parametere, `EventCallback`, routing og URL-parametere.

### Krav

- Lag den gjenbrukbare komponenten `DiceRating` som viser et terningkast fra 1 til 6. Komponenten skal ha parameterne `Value`, `ValueChanged` av typen `EventCallback<int>` og `ReadOnly`.
- `DiceRating` skal kunne brukes med `@bind-Value`. Komponenten skal aldri endre sin egen parameterverdi direkte. Den melder ny verdi til forelderen gjennom `ValueChanged`.
- Bruk `DiceRating` minst to steder: skrivebeskyttet i `GameCard` og klikkbar på spillsiden.
- Utvid `GameCard` med en `EventCallback` som sier fra til forelderen når brukeren klikker «Legg til i spillekveld». Kortet avgjør ikke selv hva som skjer. I denne oppgaven holder det at forelderen viser en bekreftelse.
- Lag spillsiden `/games/{Id:int}` som viser alle detaljene for ett spill. Brukeren skal kunne åpne spillsiden fra kortet i spillhylla.
- Når ID-en ikke finnes, viser spillsiden en tydelig melding og en lenke tilbake til spillhylla.
- Nytt terningkast på spillsiden skal vises umiddelbart. Verdien lagres først i oppgave 4.
- Lag en navigasjonsmeny med `NavLink` til forsiden, spillhylla og Min spillekveld. Aktiv lenke skal være markert. Siden Min spillekveld får innhold i oppgave 3.
- Naviger fra kode med `NavigationManager` minst ett sted, for eksempel en tilbakeknapp på spillsiden.
- Ukjente adresser skal gi en egen «Fant ikke siden»-visning med lenke til forsiden.

### Testtilfeller og kanttilfeller

- Åpne `/games/1` og en ID som ikke finnes, for eksempel `/games/9999`.
- Åpne `/games/abc`, og forklar hvorfor adressen ikke treffer spillsiden.
- Klikk terningkast 1 og 6 på spillsiden, og kontroller at forelderen får riktig verdi.
- Kontroller at `DiceRating` med `ReadOnly` ikke reagerer på klikk.
- Send en verdi utenfor 1–6 inn til `DiceRating`. Definer og dokumenter hva komponenten viser.
- Kontroller at markeringen i menyen følger siden du står på.

### Leveranse og vurderingspunkter

Lever komponentene, sidene og testresultatene, og beskriv parametere og hendelser for `DiceRating` og `GameCard` i `README.md`. Det vurderes om `DiceRating` er gjenbrukbar og brukes flere steder, om data går ned og hendelser opp, om routing og URL-parametere fungerer, og om manglende spill og ukjente adresser håndteres kontrollert.

> Tips: Tenk «data ned, hendelser opp». Forelderen eier dataene. Barnet viser dem og sier fra når noe skjer.

Kobling til læringsutbytte: gjenbrukbare komponenter, parametere, hendelser, routing, URL-parametere og komponentansvar.

## Oppgave 3 – Min spillekveld: applikasjonstilstand og responsivt grensesnitt

### Formål

La gjestene samle spill til kvelden på tvers av sidene i appen. Du skiller mellom lokal tilstand og applikasjonstilstand, styrer dataflyten bevisst og gjør grensesnittet brukbart på både mobil og stor skjerm.

### Krav

- Lag siden `/game-night` som viser spillene brukeren har valgt, antall spill og samlet spilletid.
- Brukeren skal kunne legge til og fjerne spill både fra spillhylla og fra spillsiden.
- Hold spillekvelden i en egen tilstandsklasse, for eksempel `GameNightState`, registrert som tjeneste. Tilstanden skal ikke ligge i en enkeltkomponent.
- Tilstanden skal overleve navigasjon mellom sidene.
- Vis antall valgte spill i navigasjonsmenyen. Tallet skal oppdateres straks tilstanden endres, uten at siden lastes på nytt.
- Komponenter som abonnerer på endringer i tilstanden, skal melde seg av når de fjernes, for eksempel med `IDisposable`.
- Samme spill skal ikke kunne legges til to ganger. `GameCard` viser om spillet allerede er valgt.
- Pek ut minst ett eksempel på lokal tilstand i appen, for eksempel søketeksten. Forklar hvorfor den er lokal, og hvorfor spillekvelden er applikasjonstilstand.
- Gjør grensesnittet responsivt. Spillhylla viser én kolonne på smal skjerm (ca. 375 px) og flere kolonner på bred skjerm. Menyen skal være brukbar i begge bredder.
- Vurder minst to måter å dele tilstanden på, for eksempel parametere og `EventCallback` gjennom flere nivåer, kaskaderende verdier eller en tilstandstjeneste. Begrunn valget ditt.
- Begrunn levetiden til tilstandstjenesten, scoped eller singleton. Forklar hva som skjer når to brukere eller to faner bruker appen samtidig.

### Testtilfeller og kanttilfeller

- Legg til et spill fra spillhylla, og kontroller at tallet i menyen øker.
- Gå til en spillside og tilbake, og kontroller at spillekvelden er uendret.
- Forsøk å legge til samme spill to ganger.
- Fjern det siste spillet, og kontroller at siden viser en tom-tilstand og at samlet spilletid er 0.
- Kontroller samlet spilletid med minst tre spill.
- Åpne appen i to faner, og dokumenter om spillekvelden deles mellom dem. Stemmer resultatet med levetiden du valgte?
- Vis spillhylla i ca. 375 px og minst 1200 px bredde, og legg skjermbilder i `README.md`.

### Leveranse og vurderingspunkter

Lever tilstandsklassen, sidene, testresultatene og skjermbildene, og beskriv dataflyten og vurderingen av alternativer i `README.md`. Det vurderes om tilstanden har ett tydelig hjem, om grensesnittet oppdateres når tilstanden endres, om oppsettet fungerer på smal og bred skjerm, og om valget mellom designalternativene er faglig begrunnet.

> Tips: Forsvinner tilstanden når du navigerer? Sjekk gjengivelsesmodus, om interaktiviteten er global eller per side, og levetiden til tjenesten.

Kobling til læringsutbytte: lokal tilstand, applikasjonstilstand, dataflyt, responsive grensesnitt, vurdering av designalternativer og vedlikeholdbar struktur.

## Oppgave 4 – Bak disken: HttpClient og CRUD mot REST API

### Formål

Bytt ut den faste listen med et REST API, og gi de ansatte en side for å vedlikeholde samlingen. Du bruker asynkrone kall med `HttpClient`, JSON, komponentlivssyklus og et bevisst valg av gjengivelsesmodus.

### Krav

- Lag et enkelt REST API for spillene i samme løsning, som minimal API eller controller. Lagring i minnet er nok, og database er ikke et krav. API-et starter med minst åtte spill.
- API-et skal ha endepunktene i tabellen under.
- Samle all kommunikasjon med API-et i én tjenesteklasse, for eksempel `GameApiClient`, som får `HttpClient` gjennom dependency injection. Ingen komponent skal opprette `HttpClient` selv eller kjenne adressene til API-et.
- Bruk `async` og `await` i alle kall. Ikke blokker med `.Result` eller `.Wait()`.
- Hent data i riktig livssyklusmetode. Forklar valget på spillhylla, og forklar hva som må til på spillsiden når `Id` i adressen endres.
- Vis tre tilstander på sider som henter data: laster, feil og tom liste.
- Lag siden `/admin/games` der ansatte kan opprette, endre og slette spill. Bruk et skjema med databinding og validering, for eksempel `EditForm`.
- Valider minst dette: navn er påkrevd, `MinPlayers` er minst 1, `MaxPlayers` er ikke lavere enn `MinPlayers`, spilletiden er over 0, og terningkastet er fra 1 til 6.
- Be om bekreftelse før et spill slettes.
- Listen skal vise endringene etter oppretting, endring og sletting uten at brukeren laster siden på nytt.
- Terningkast gitt på spillsiden skal nå lagres via API-et.
- Velg gjengivelsesmodus for appen, og begrunn valget. Forklar hvor koden kjører, og nevn minst én ting som ville blitt annerledes med en annen modus.

| Metode | Rute              | Svar                                                            |
|--------|-------------------|-----------------------------------------------------------------|
| GET    | `/api/games`      | 200 med alle spill                                              |
| GET    | `/api/games/{id}` | 200 med spillet, 404 når ID-en ikke finnes                      |
| POST   | `/api/games`      | 201 med det nye spillet, 400 ved ugyldige data                  |
| PUT    | `/api/games/{id}` | 204 eller 200, 400 ved ugyldige data, 404 når ID-en ikke finnes |
| DELETE | `/api/games/{id}` | 204, 404 når ID-en ikke finnes                                  |

### Testtilfeller og kanttilfeller

- Stopp API-et og åpne spillhylla. Appen skal vise en feilmelding og ikke krasje.
- Opprett et spill med tomt navn, og et spill der `MaxPlayers` er lavere enn `MinPlayers`. Skjemaet skal vise valideringsfeil.
- Send ugyldige data rett til API-et, for eksempel fra en `.http`-fil, og kontroller at svaret er 400.
- Endre et spill, og kontroller at både spillhylla og spillsiden viser endringen.
- Slett et spill som ligger i Min spillekveld. Definer og dokumenter hva som skal skje.
- Åpne spillsiden for en ID som API-et svarer 404 på.
- Legg en lenke til et annet spill på spillsiden, for eksempel «Neste spill». Kontroller at riktig spill vises når `Id` endres.

### Leveranse og vurderingspunkter

Lever API-et, tjenesteklassen, adminsiden og testresultatene, og forklar livssyklus, feilhåndtering og gjengivelsesmodus i `README.md`. Det vurderes om alle fire CRUD-operasjonene fungerer mot API-et, om kallene er asynkrone og samlet i en tjeneste, om laste-, feil- og tom-tilstand er håndtert, og om valgene er faglig begrunnet.

> Tips: `OnInitializedAsync` kan kjøre to ganger når forhåndsrendering er slått på. Se etter doble kall i loggen, og forklar det du ser.

Kobling til læringsutbytte: asynkrone kall med `HttpClient`, JSON, CRUD mot REST API, komponentlivssyklus, gjengivelsesmodi og komplekse problemstillinger.

## Oppgave 5 – Designdokumentasjon, testing og designvalg

### Formål

Dokumenter designet med enkle modeller, test de sentrale delene, og begrunn valgene dine. En annen utvikler skal kunne forstå hvordan appen henger sammen uten å lese all koden.

### Krav til designdokumentasjon

Legg dokumentasjonen i `README.md` eller i en egen fil, for eksempel `docs/design.md`.

- Sideoversikt: en tabell med rute, sidekomponent og formål.
- Komponenttre: et diagram over sider og komponenter som viser parametere ned og hendelser opp.
- Dataflyt: et diagram for én brukerhandling fra klikk til oppdatert skjerm, for eksempel «legg til i spillekveld» eller «lagre nytt spill».
- Ansvarsliste: én setning om ansvaret til hver komponent og tjeneste.

Diagrammene kan lages i Mermaid, draw.io eller for hånd. Innholdet teller, ikke verktøyet.

### Krav til designvalg

- Dokumenter minst tre designvalg. For hvert valg beskriver du problemet, minst to alternativer, valget ditt og hva valget koster.
- To av valgene skal handle om deling av tilstand og gjengivelsesmodus. Det tredje velger du selv.
- Beskriv hva som ville vært enkelt og hva som ville vært vanskelig å endre i løsningen. Bruk minst to eksempler, som å bytte lagring i minnet med en database eller å legge til et nytt felt på spillet.
- Beskriv kjente begrensninger.

### Krav til testing

- Skriv minst fem automatiserte tester i et eget testprosjekt.
- Minst tre tester skal dekke logikk utenfor komponentene, for eksempel `GameNightState`, filtreringen eller valideringen.
- Minst én test skal teste en komponent, for eksempel at `DiceRating` sender riktig verdi gjennom `ValueChanged`. Bruk for eksempel bUnit.
- Testene skal dekke både normal bruk og kanttilfeller.
- Før en manuell testlogg for testtilfellene i oppgave 1–4 som ikke er automatisert. Loggen viser testtilfelle, steg, forventet resultat og faktisk resultat.

### Leveranse og vurderingspunkter

Lever designdokumentasjonen, testprosjektet og testloggen. Det vurderes om diagrammene stemmer med koden, om designvalgene veier reelle alternativer mot hverandre, om testene er systematiske og dekker kanttilfeller, og om refleksjonen viser forståelse for vedlikeholdbarhet.

> Tips: Tegn komponenttreet før du koder oppgave 2, og oppdater det underveis. Et enkelt diagram som stemmer med koden, er verdt mer enn et pent diagram som ikke gjør det.

Kobling til læringsutbytte: modellering og dokumentasjon av software design, testing, faglig begrunnede designvalg og vedlikeholdbar kode.

## Oppgave 6 – Videopresentasjon og refleksjon

### Formål

Vis at du forstår og kan formidle din egen løsning. Presentasjonen skal primært handle om hvordan appen er bygd opp, og hvorfor du har valgt designet du har.

Hvis du ikke har fått løst alle oppgavene, viser og forklarer du de delene du har løst.

### Krav

- Lag en video på 5–8 minutter med skjermdeling og tydelig lyd. Webcam er valgfritt.
- Bruk norsk eller engelsk, og bruk samme språk gjennom hele presentasjonen.
- Vis appen i bruk, fra spillhylla til Min spillekveld og adminsiden, på både bred og smal skjerm.
- Forklar komponentstrukturen med utgangspunkt i komponenttreet ditt.
- Vis én sentral kodebit, for eksempel tilstandsklassen eller API-tjenesten, og forklar hvordan data flyter gjennom den.
- Vis minst ett normalt og ett avvikende tilfelle, for eksempel at API-et er nede eller at et spill ikke finnes.
- Forklar ett designvalg der du vurderte flere alternativer.
- Beskriv én utfordring du møtte, og hvordan du arbeidet med den.
- Reflekter over forbedringer, videre arbeid og KI-bruk.

### Leveranse og vurderingspunkter

Legg ved videofil eller en tilgjengelig videolenke i ZIP-filen. Det vurderes om videoen oppfyller tidsrammen, om den viser og forklarer egen kode og kjøring, og om refleksjonen viser forståelse av designvalg, tester, begrensninger og KI-bruk.

Kobling til læringsutbytte: faglig begrunnelse, refleksjon over egne designvalg og evne til å arbeide med komplekse problemstillinger.

## Vurdering: Godkjent / Ikke godkjent

Arbeidskravet vurderes helhetlig. Alle oppgavene skal være levert eller reelt forsøkt. Mindre feil eller delvis fungerende kode fører ikke alene til Ikke godkjent når du ellers viser relevant faglig forståelse og kan begrunne løsningene.

| Område                               | Godkjent                                                                                                                                                       | Ikke godkjent                                                                                                 |
|--------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------|
| Fullførelse og kjørbarhet            | Alle oppgaveområder og video er levert eller reelt forsøkt, og løsningen er i hovedsak kjørbar eller godt dokumentert.                                         | Vesentlige deler mangler, eller materialet kan ikke vurderes.                                                 |
| Komponenter og databinding           | Grensesnittet er delt i egne komponenter med tydelig ansvar, og databinding og hendelser brukes riktig i søk og filtrering.                                    | Appen er i hovedsak én stor komponent, eller sentral interaktivitet mangler.                                  |
| Komponentkommunikasjon og navigasjon | `DiceRating` er gjenbrukbar og kommuniserer med parametere og `EventCallback`. Routing og URL-parametere fungerer, også når spillet ikke finnes.               | Gjenbrukbar komponent, kommunikasjon mellom komponenter eller routing mangler i vesentlig grad.               |
| Tilstand og responsivt grensesnitt   | Spillekvelden holdes i en egen tilstandsklasse, grensesnittet oppdateres når tilstanden endres, og appen er brukbar på smal og bred skjerm.                    | Delt tilstand mangler eller fungerer ikke på tvers av sider, eller grensesnittet er ubrukelig på smal skjerm. |
| HTTP, CRUD og livssyklus             | CRUD mot REST API fungerer med asynkrone kall samlet i en tjeneste. Laste-, feil- og tom-tilstand er håndtert, og livssyklus og gjengivelsesmodus er forklart. | Sentrale CRUD-operasjoner mangler, data hentes ikke fra API-et, eller løsningen kan ikke forklares.           |
| Design og begrunnelser               | Diagrammene stemmer med koden, og minst tre designvalg er begrunnet med reelle alternativer.                                                                   | Designdokumentasjon eller begrunnelser mangler i vesentlig grad.                                              |
| Kodekvalitet og testarbeid           | Koden er ryddig, har beskrivende navn og skiller grensesnitt fra data og logikk. Automatiserte tester og testlogg dekker normal bruk og kanttilfeller.         | Koden eller testgrunnlaget er så uoversiktlig eller mangelfullt at forståelse og funksjon ikke kan vurderes.  |
| Dokumentasjon, Git og video          | `README.md`, relevant Git-historikk, dokumentasjon og video gir et troverdig bilde av eget arbeid.                                                             | Dokumentasjon, Git eller video mangler i vesentlig grad.                                                      |
| KI-refleksjon og eierskap            | Refleksjon over KI-bruk er levert, eller det er opplyst at KI ikke er brukt. Studenten kan gjøre rede for arbeidet ved behov.                                  | Refleksjon over KI-bruk mangler når KI er brukt, eller eierskapet til arbeidet kan ikke sannsynliggjøres.     |

For at arbeidskravet skal bli godkjent, må innleveringen samlet sett gi tilstrekkelig grunnlag for å vurdere at du har arbeidet med og viser relevant kompetanse i modul 2.