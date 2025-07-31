using NUnit.Framework;

//namespaces used
using Game.Core;

public class HealthTests
{
    [Test]
    public void TakeDamage_ReducesHealthCorrectly() {
        var health = new Health(100);
        health.TakeDamage(25);
        Assert.AreEqual(75, health.currentHealth);
    }

    [Test]
    public void TakeDamage_NeverBelowZero() {
        var health = new Health(50);
        health.TakeDamage(100);
        Assert.AreEqual(0, health.currentHealth);
        Assert.IsTrue(health.isDead);
    }
    [Test]
    public void Heal_RestoresHealthButNotAboveMax() {
        var health = new Health(100);
        health.TakeDamage(50);
        health.Heal(80);

        Assert.AreEqual(100, health.currentHealth); // not 130
    }
}
