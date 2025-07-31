using NUnit.Framework;

public class StaminaSystemTests
{
    [Test]
    public void Sprinting_DrainsStamina() {
        var stamina = new StaminaSystem(100f, 10f, 5f);
        stamina.Update(true, 1f);
        Assert.AreEqual(90f, stamina.currentStamina);
    }

    [Test]
    public void Resting_RegeneratesStamina() {
        var stamina = new StaminaSystem(100f, 10f, 5f);
        stamina.Update(true, 2f);
        stamina.Update(false, 2f);
        Assert.AreEqual(90f, stamina.currentStamina);
    }

    [Test]
    public void Stamina_StopsAtZero() {
        var stamina = new StaminaSystem(50f, 100f, 0f);
        stamina.Update(true, 1f);
        Assert.AreEqual(0f, stamina.currentStamina);
        Assert.IsTrue(stamina.isExhausted);
    }
}
