using Microsoft.VisualStudio.TestTools.UnitTesting;
using shared.Model;

namespace ordination_test;

[TestClass]
public class ModelKlasserTest
{
    //
    // GYLDIGE TESTS
    //

    [TestMethod]
    public void DoegnDosis_KorrektNårAlleDoserErPositive()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");
        var ord = new DagligFast(DateTime.Today, DateTime.Today, lm, 2, 1, 1, 1);

        double resultat = ord.doegnDosis();

        Assert.AreEqual(5, resultat); // 2+1+1+1
    }

    [TestMethod]
    public void DoegnDosis_KorrektNårAlleDoserErNul()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");
        var ord = new DagligFast(DateTime.Today, DateTime.Today, lm, 0, 0, 0, 0);

        double resultat = ord.doegnDosis();

        Assert.AreEqual(0, resultat);
    }

    [TestMethod]
    public void SamletDosis_EnDag()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");
        DateTime d = new DateTime(2021, 1, 1);

        var ord = new DagligFast(d, d, lm, 2, 1, 1, 0);

        double resultat = ord.samletDosis(); // doegnDosis = 4, antalDage = 1

        Assert.AreEqual(4, resultat);
    }

    [TestMethod]
    public void SamletDosis_FlereDage()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");

        var ord = new DagligFast(
            new DateTime(2021, 1, 1),
            new DateTime(2021, 1, 3),
            lm,
            2, 1, 1, 0
        );

        // doegnDosis = 4
        // antalDage = 3
        // forventet = 4 * 3 = 12
        double resultat = ord.samletDosis();

        Assert.AreEqual(12, resultat);
    }

    //
    // UGYLDIGE TESTS (forventede fejl)
    //

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AntalDage_KasterFejlNårSlutdatoErFørStartdato()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");

        var ord = new DagligFast(
            new DateTime(2021, 1, 10),
            new DateTime(2021, 1, 5),   // fejl: slut < start
            lm,
            1, 1, 1, 1
        );

        ord.antalDage(); // Skal kaste exception
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void NegativeDoser_KasterFejl()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");

        var ord = new DagligFast(
            DateTime.Today,
            DateTime.Today,
            lm,
            -1, 0, 0, 0 // negativ dosis → ugyldig
        );
    }

   
    [TestMethod]
    public void SamletDosis_EnDosisEnDag_Giver10()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");
        var pn = new PN(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10), 10, lm);

        pn.dates.Add(new Dato { dato = new DateTime(2021, 1, 1) });

        double resultat = pn.samletDosis();

        Assert.AreEqual(10, resultat);
    }
    [TestMethod]
    public void PNSamletDosis_TreDage_Giver15()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");
        var pn = new PN(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10), 5, lm);

        pn.dates.Add(new Dato { dato = new DateTime(2021, 1, 1) });
        pn.dates.Add(new Dato { dato = new DateTime(2021, 1, 2) });
        pn.dates.Add(new Dato { dato = new DateTime(2021, 1, 3) });

        double resultat = pn.samletDosis();

        Assert.AreEqual(15, resultat);
    }
    [TestMethod]
    public void PNSamletDosis_AntalEnheder0_Giver0()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");
        var pn = new PN(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10), 0, lm);

        pn.dates.Add(new Dato { dato = new DateTime(2021, 1, 1) });

        double resultat = pn.samletDosis();

        Assert.AreEqual(0, resultat);
    }
    [TestMethod]
    public void PNGivDosis_DatoUdenforPeriode_GiverFalse()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");
        var pn = new PN(new DateTime(2021, 1, 2), new DateTime(2021, 1, 5), 3, lm);

        // Dato før start
        bool resultat = pn.givDosis(new Dato { dato = new DateTime(2021, 1, 1) });

        Assert.IsFalse(resultat);
    }

    [TestMethod]
    public void PNGivDosis_DatoErStartdato_GiverTrue()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");
        var pn = new PN(new DateTime(2021, 1, 2), new DateTime(2021, 1, 5), 3, lm);

        bool resultat = pn.givDosis(new Dato { dato = new DateTime(2021, 1, 2) });

        Assert.IsTrue(resultat);
    }
    [TestMethod]
    public void PNGivDosis_DatoEfterSlutdato_GiverFalse()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");
        var pn = new PN(new DateTime(2021, 1, 2), new DateTime(2021, 1, 5), 3, lm);

        bool resultat = pn.givDosis(new Dato { dato = new DateTime(2021, 1, 6) });

        Assert.IsFalse(resultat);
    }
    [TestMethod]
    public void PNSamletDosis_IngenDage_Giver0()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");
        var pn = new PN(new DateTime(2021, 1, 1), new DateTime(2021, 1, 10), 4, lm);

        // ingen datoer tilføjet → 0 dage

        double resultat = pn.samletDosis();

        Assert.AreEqual(0, resultat);
    }
    [TestMethod]
[ExpectedException(typeof(ArgumentException))]
public void PNGivDosis_DatoUdenforPeriode_KasterFejl()
{
    var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");
    var pn = new PN(new DateTime(2021, 1, 1), new DateTime(2021, 1, 3), 3, lm);

    // 05.01 ligger UDEN for perioden
    pn.givDosis(new Dato { dato = new DateTime(2021, 1, 5) });
}
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PN_NegativAntalEnheder_KasterFejl()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");

        var pn = new PN(new DateTime(2021, 1, 1), new DateTime(2021, 1, 3), -2, lm);
    }


    [TestMethod]
    public void AntalDage_13Dage_Korrekt()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");

        var ord = new DagligFast(
            new DateTime(2021, 12, 31),
            new DateTime(2022, 1, 12),
            lm, 1, 1, 1, 1
        );

        int resultat = ord.antalDage();

        Assert.AreEqual(13, resultat);
    }
    [TestMethod]
    public void AntalDage_2Dage_Korrekt()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");

        var ord = new DagligFast(
            new DateTime(2021, 12, 31),
            new DateTime(2022, 1, 1),
            lm, 1, 1, 1, 1
        );

        int resultat = ord.antalDage();

        Assert.AreEqual(2, resultat);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AntalDage_KasterFejl_NaarSlutDatoErFoerStartDato()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");

        var ord = new DagligFast(
            new DateTime(2022, 1, 12),
            new DateTime(2021, 12, 31),
            lm, 1, 1, 1, 1
        );

        ord.antalDage(); // Skal kaste exception
    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AntalDage_KasterFejl_Igen()
    {
        var lm = new Laegemiddel("Test", 1, 1, 1, "Styk");

        var ord = new DagligFast(
            new DateTime(2022, 1, 10),
            new DateTime(2021, 12, 31),
            lm, 1, 1, 1, 1
        );

        ord.antalDage(); // Skal kaste exception
    }

}
