namespace shared.Model;

public class PN : Ordination {
	public double antalEnheder { get; set; }
    public List<Dato> dates { get; set; } = new List<Dato>();

    public PN (DateTime startDen, DateTime slutDen, double antalEnheder, Laegemiddel laegemiddel) : base(laegemiddel, startDen, slutDen) {
		this.antalEnheder = antalEnheder;
	}

    public PN() : base(null!, new DateTime(), new DateTime()) {
    }

    /// <summary>
    /// Registrerer at der er givet en dosis på dagen givesDen
    /// Returnerer true hvis givesDen er inden for ordinationens gyldighedsperiode og datoen huskes
    /// Returner false ellers og datoen givesDen ignoreres
    /// </summary>
    public bool givDosis(Dato givesDen) 
        {
            // Hvis datoen er før start, må medicin ikke gives
            if (givesDen.dato < startDen)
            {
                return false;
            }

            // Hvis datoen er efter slut, må medicin ikke gives
            if (givesDen.dato > slutDen)
            {
                return false;
            }

        // Hvis datoen er gyldig, registrer at medicinen er givet
        dates.Add(givesDen);


        // Returnér true: medicinen blev givet korrekt
        return true;
        }

    public override double doegnDosis()
    {
        // Hvis der aldrig er givet medicin, er døgndosis 0
        if (dates.Count == 0)
            return 0;

        // Find første og sidste dato i listen
        DateTime første = dates.Min(d => d.dato);
        DateTime sidste = dates.Max(d => d.dato);

        // Beregn antal dage inklusiv begge dage
        int antalDage = (sidste - første).Days + 1;

        // Hvor mange gange medicinen er givet
        int antalGange = dates.Count;

        // Totalt antal enheder = (antal gange givet) * (antal enheder pr. gang)
        double totalEnheder = antalGange * antalEnheder;

        // Døgndosis = totalEnheder fordelt over antalDage
        return totalEnheder / antalDage;
    }



    public override double samletDosis() {
        return dates.Count() * antalEnheder;
    }

    public int getAntalGangeGivet() {
        return dates.Count();
    }

	public override String getType() {
		return "PN";
	}
}
