using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace AOC2019.Tests
{
    /*
     * --- Day 12: The N-Body Problem ---
     *
     * The space near Jupiter is not a very safe place; you need to be
     * careful of a big distracting red spot, extreme radiation, and a
     * whole lot of moons swirling around. You decide to start by
     * tracking the four largest moons: Io, Europa, Ganymede, and
     * Callisto.
     *
     * After a brief scan, you calculate the position of each moon (your
     * puzzle input). You just need to simulate their motion so you can
     * avoid them.
     *
     * Each moon has a 3-dimensional position (x, y, and z) and a
     * 3-dimensional velocity. The position of each moon is given in
     * your scan; the x, y, and z velocity of each moon starts at 0.
     *
     * Simulate the motion of the moons in time steps. Within each time
     * step, first update the velocity of every moon by applying
     * gravity. Then, once all moons' velocities have been updated,
     * update the position of every moon by applying velocity. Time
     * progresses by one step once all of the positions are updated.
     *
     * To apply gravity, consider every pair of moons. On each axis
     * (x, y, and z), the velocity of each moon changes by exactly +1 or
     * -1 to pull the moons together. For example, if Ganymede has an x
     * position of 3, and Callisto has a x position of 5, then
     * Ganymede's x velocity changes by +1 (because 5 > 3) and
     * Callisto's x velocity changes by -1 (because 3 < 5). However, if
     * the positions on a given axis are the same, the velocity on that
     * axis does not change for that pair of moons.
     *
     * Once all gravity has been applied, apply velocity: simply add the
     * velocity of each moon to its own position. For example, if Europa
     * has a position of x=1, y=2, z=3 and a velocity of x=-2, y=0,z=3,
     * then its new position would be x=-1, y=2, z=6. This process does
     * not modify the velocity of any moon.
     *
     * For example, suppose your scan reveals the following positions:
     *
     * <x=-1, y=0, z=2>
     * <x=2, y=-10, z=-7>
     * <x=4, y=-8, z=8>
     * <x=3, y=5, z=-1>
     *
     * Simulating the motion of these moons would produce the following:
     *
     * After 0 steps:
     * pos=<x=-1, y=  0, z= 2>, vel=<x= 0, y= 0, z= 0>
     * pos=<x= 2, y=-10, z=-7>, vel=<x= 0, y= 0, z= 0>
     * pos=<x= 4, y= -8, z= 8>, vel=<x= 0, y= 0, z= 0>
     * pos=<x= 3, y=  5, z=-1>, vel=<x= 0, y= 0, z= 0>
     *
     * After 1 step:
     * pos=<x= 2, y=-1, z= 1>, vel=<x= 3, y=-1, z=-1>
     * pos=<x= 3, y=-7, z=-4>, vel=<x= 1, y= 3, z= 3>
     * pos=<x= 1, y=-7, z= 5>, vel=<x=-3, y= 1, z=-3>
     * pos=<x= 2, y= 2, z= 0>, vel=<x=-1, y=-3, z= 1>
     *
     * After 2 steps:
     * pos=<x= 5, y=-3, z=-1>, vel=<x= 3, y=-2, z=-2>
     * pos=<x= 1, y=-2, z= 2>, vel=<x=-2, y= 5, z= 6>
     * pos=<x= 1, y=-4, z=-1>, vel=<x= 0, y= 3, z=-6>
     * pos=<x= 1, y=-4, z= 2>, vel=<x=-1, y=-6, z= 2>
     *
     * After 3 steps:
     * pos=<x= 5, y=-6, z=-1>, vel=<x= 0, y=-3, z= 0>
     * pos=<x= 0, y= 0, z= 6>, vel=<x=-1, y= 2, z= 4>
     * pos=<x= 2, y= 1, z=-5>, vel=<x= 1, y= 5, z=-4>
     * pos=<x= 1, y=-8, z= 2>, vel=<x= 0, y=-4, z= 0>
     *
     * After 4 steps:
     * pos=<x= 2, y=-8, z= 0>, vel=<x=-3, y=-2, z= 1>
     * pos=<x= 2, y= 1, z= 7>, vel=<x= 2, y= 1, z= 1>
     * pos=<x= 2, y= 3, z=-6>, vel=<x= 0, y= 2, z=-1>
     * pos=<x= 2, y=-9, z= 1>, vel=<x= 1, y=-1, z=-1>
     *
     * After 5 steps:
     * pos=<x=-1, y=-9, z= 2>, vel=<x=-3, y=-1, z= 2>
     * pos=<x= 4, y= 1, z= 5>, vel=<x= 2, y= 0, z=-2>
     * pos=<x= 2, y= 2, z=-4>, vel=<x= 0, y=-1, z= 2>
     * pos=<x= 3, y=-7, z=-1>, vel=<x= 1, y= 2, z=-2>
     *
     * After 6 steps:
     * pos=<x=-1, y=-7, z= 3>, vel=<x= 0, y= 2, z= 1>
     * pos=<x= 3, y= 0, z= 0>, vel=<x=-1, y=-1, z=-5>
     * pos=<x= 3, y=-2, z= 1>, vel=<x= 1, y=-4, z= 5>
     * pos=<x= 3, y=-4, z=-2>, vel=<x= 0, y= 3, z=-1>
     *
     * After 7 steps:
     * pos=<x= 2, y=-2, z= 1>, vel=<x= 3, y= 5, z=-2>
     * pos=<x= 1, y=-4, z=-4>, vel=<x=-2, y=-4, z=-4>
     * pos=<x= 3, y=-7, z= 5>, vel=<x= 0, y=-5, z= 4>
     * pos=<x= 2, y= 0, z= 0>, vel=<x=-1, y= 4, z= 2>
     *
     * After 8 steps:
     * pos=<x= 5, y= 2, z=-2>, vel=<x= 3, y= 4, z=-3>
     * pos=<x= 2, y=-7, z=-5>, vel=<x= 1, y=-3, z=-1>
     * pos=<x= 0, y=-9, z= 6>, vel=<x=-3, y=-2, z= 1>
     * pos=<x= 1, y= 1, z= 3>, vel=<x=-1, y= 1, z= 3>
     *
     * After 9 steps:
     * pos=<x= 5, y= 3, z=-4>, vel=<x= 0, y= 1, z=-2>
     * pos=<x= 2, y=-9, z=-3>, vel=<x= 0, y=-2, z= 2>
     * pos=<x= 0, y=-8, z= 4>, vel=<x= 0, y= 1, z=-2>
     * pos=<x= 1, y= 1, z= 5>, vel=<x= 0, y= 0, z= 2>
     *
     * After 10 steps:
     * pos=<x= 2, y= 1, z=-3>, vel=<x=-3, y=-2, z= 1>
     * pos=<x= 1, y=-8, z= 0>, vel=<x=-1, y= 1, z= 3>
     * pos=<x= 3, y=-6, z= 1>, vel=<x= 3, y= 2, z=-3>
     * pos=<x= 2, y= 0, z= 4>, vel=<x= 1, y=-1, z=-1>
     *
     * Then, it might help to calculate the total energy in the system.
     * The total energy for a single moon is its potential energy
     * multiplied by its kinetic energy. A moon's potential energy is
     * the sum of the absolute values of its x, y, and z position
     * coordinates. A moon's kinetic energy is the sum of the absolute
     * values of its velocity coordinates. Below, each line shows the
     * calculations for a moon's potential energy (pot), kinetic energy
     * (kin), and total energy:
     *
     * Energy after 10 steps:
     * pot: 2 + 1 + 3 =  6;   kin: 3 + 2 + 1 = 6;   total:  6 * 6 = 36
     * pot: 1 + 8 + 0 =  9;   kin: 1 + 1 + 3 = 5;   total:  9 * 5 = 45
     * pot: 3 + 6 + 1 = 10;   kin: 3 + 2 + 3 = 8;   total: 10 * 8 = 80
     * pot: 2 + 0 + 4 =  6;   kin: 1 + 1 + 1 = 3;   total:  6 * 3 = 18
     *
     * Sum of total energy: 36 + 45 + 80 + 18 = 179
     *
     * In the above example, adding together the total energy for all
     * moons after 10 steps produces the total energy in the system,
     * 179.
     *
     * Here's a second example:
     *
     * <x=-8, y=-10, z=0>
     * <x=5, y=5, z=10>
     * <x=2, y=-7, z=3>
     * <x=9, y=-8, z=-3>
     *
     * Every ten steps of simulation for 100 steps produces:
     *
     * After 0 steps:
     * pos=<x= -8, y=-10, z=  0>, vel=<x=  0, y=  0, z=  0>
     * pos=<x=  5, y=  5, z= 10>, vel=<x=  0, y=  0, z=  0>
     * pos=<x=  2, y= -7, z=  3>, vel=<x=  0, y=  0, z=  0>
     * pos=<x=  9, y= -8, z= -3>, vel=<x=  0, y=  0, z=  0>
     *
     * After 10 steps:
     * pos=<x= -9, y=-10, z=  1>, vel=<x= -2, y= -2, z= -1>
     * pos=<x=  4, y= 10, z=  9>, vel=<x= -3, y=  7, z= -2>
     * pos=<x=  8, y=-10, z= -3>, vel=<x=  5, y= -1, z= -2>
     * pos=<x=  5, y=-10, z=  3>, vel=<x=  0, y= -4, z=  5>
     *
     * After 20 steps:
     * pos=<x=-10, y=  3, z= -4>, vel=<x= -5, y=  2, z=  0>
     * pos=<x=  5, y=-25, z=  6>, vel=<x=  1, y=  1, z= -4>
     * pos=<x= 13, y=  1, z=  1>, vel=<x=  5, y= -2, z=  2>
     * pos=<x=  0, y=  1, z=  7>, vel=<x= -1, y= -1, z=  2>
     *
     * After 30 steps:
     * pos=<x= 15, y= -6, z= -9>, vel=<x= -5, y=  4, z=  0>
     * pos=<x= -4, y=-11, z=  3>, vel=<x= -3, y=-10, z=  0>
     * pos=<x=  0, y= -1, z= 11>, vel=<x=  7, y=  4, z=  3>
     * pos=<x= -3, y= -2, z=  5>, vel=<x=  1, y=  2, z= -3>
     *
     * After 40 steps:
     * pos=<x= 14, y=-12, z= -4>, vel=<x= 11, y=  3, z=  0>
     * pos=<x= -1, y= 18, z=  8>, vel=<x= -5, y=  2, z=  3>
     * pos=<x= -5, y=-14, z=  8>, vel=<x=  1, y= -2, z=  0>
     * pos=<x=  0, y=-12, z= -2>, vel=<x= -7, y= -3, z= -3>
     *
     * After 50 steps:
     * pos=<x=-23, y=  4, z=  1>, vel=<x= -7, y= -1, z=  2>
     * pos=<x= 20, y=-31, z= 13>, vel=<x=  5, y=  3, z=  4>
     * pos=<x= -4, y=  6, z=  1>, vel=<x= -1, y=  1, z= -3>
     * pos=<x= 15, y=  1, z= -5>, vel=<x=  3, y= -3, z= -3>
     *
     * After 60 steps:
     * pos=<x= 36, y=-10, z=  6>, vel=<x=  5, y=  0, z=  3>
     * pos=<x=-18, y= 10, z=  9>, vel=<x= -3, y= -7, z=  5>
     * pos=<x=  8, y=-12, z= -3>, vel=<x= -2, y=  1, z= -7>
     * pos=<x=-18, y= -8, z= -2>, vel=<x=  0, y=  6, z= -1>
     *
     * After 70 steps:
     * pos=<x=-33, y= -6, z=  5>, vel=<x= -5, y= -4, z=  7>
     * pos=<x= 13, y= -9, z=  2>, vel=<x= -2, y= 11, z=  3>
     * pos=<x= 11, y= -8, z=  2>, vel=<x=  8, y= -6, z= -7>
     * pos=<x= 17, y=  3, z=  1>, vel=<x= -1, y= -1, z= -3>
     *
     * After 80 steps:
     * pos=<x= 30, y= -8, z=  3>, vel=<x=  3, y=  3, z=  0>
     * pos=<x= -2, y= -4, z=  0>, vel=<x=  4, y=-13, z=  2>
     * pos=<x=-18, y= -7, z= 15>, vel=<x= -8, y=  2, z= -2>
     * pos=<x= -2, y= -1, z= -8>, vel=<x=  1, y=  8, z=  0>
     *
     * After 90 steps:
     * pos=<x=-25, y= -1, z=  4>, vel=<x=  1, y= -3, z=  4>
     * pos=<x=  2, y= -9, z=  0>, vel=<x= -3, y= 13, z= -1>
     * pos=<x= 32, y= -8, z= 14>, vel=<x=  5, y= -4, z=  6>
     * pos=<x= -1, y= -2, z= -8>, vel=<x= -3, y= -6, z= -9>
     *
     * After 100 steps:
     * pos=<x=  8, y=-12, z= -9>, vel=<x= -7, y=  3, z=  0>
     * pos=<x= 13, y= 16, z= -3>, vel=<x=  3, y=-11, z= -5>
     * pos=<x=-29, y=-11, z= -1>, vel=<x= -3, y=  7, z=  4>
     * pos=<x= 16, y=-13, z= 23>, vel=<x=  7, y=  1, z=  1>
     *
     * Energy after 100 steps:
     * pot:  8 + 12 +  9 = 29;   kin: 7 +  3 + 0 = 10;   total: 290
     * pot: 13 + 16 +  3 = 32;   kin: 3 + 11 + 5 = 19;   total: 608
     * pot: 29 + 11 +  1 = 41;   kin: 3 +  7 + 4 = 14;   total: 574
     * pot: 16 + 13 + 23 = 52;   kin: 7 +  1 + 1 =  9;   total: 468
     *
     * Sum of total energy: 290 + 608 + 574 + 468 = 1940
     *
     * What is the total energy in the system after simulating the moons
     * given in your scan for 1000 steps?
     *
     * Your puzzle answer was 10664.
     */
    public class Day12Tests : TestBase
    {
        public Day12Tests() : base(12)
        {
        }

        [Test]
        public void Example_Parsing()
        {
            var input =
                "<x=-1, y=0, z=2>\n<x=2, y=-10, z=-7>\n<x=4, y=-8, z=8>\n<x=3, y=5, z=-1>"
                    .Split('\n');

            new Day12(input).Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(-1, 0, 2, 0, 0, 0),
                    new Day12.Moon(2, -10, -7, 0, 0, 0),
                    new Day12.Moon(4, -8, 8, 0, 0, 0),
                    new Day12.Moon(3, 5, -1, 0, 0, 0)
                });
        }

        [Test]
        public void Example1()
        {
            var input =
                "<x=-1, y=0, z=2>\n<x=2, y=-10, z=-7>\n<x=4, y=-8, z=8>\n<x=3, y=5, z=-1>"
                    .Split('\n');

            var day12 = new Day12(input);

            day12.TakeStep();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(2, -1, 1, 3, -1, -1),
                    new Day12.Moon(3, -7, -4, 1, 3, 3),
                    new Day12.Moon(1, -7, 5, -3, 1, -3),
                    new Day12.Moon(2, 2, 0, -1, -3, 1)
                });

            day12.TakeStep();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(5, -3, -1, 3, -2, -2),
                    new Day12.Moon(1, -2, 2, -2, 5, 6),
                    new Day12.Moon(1, -4, -1, 0, 3, -6),
                    new Day12.Moon(1, -4, 2, -1, -6, 2)
                });

            day12.TakeStep();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(5, -6, -1, 0, -3, 0),
                    new Day12.Moon(0, 0, 6, -1, 2, 4),
                    new Day12.Moon(2, 1, -5, 1, 5, -4),
                    new Day12.Moon(1, -8, 2, 0, -4, 0)
                });

            day12.TakeStep();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(2, -8, 0, -3, -2, 1),
                    new Day12.Moon(2, 1, 7, 2, 1, 1),
                    new Day12.Moon(2, 3, -6, 0, 2, -1),
                    new Day12.Moon(2, -9, 1, 1, -1, -1)
                });

            day12.TakeStep();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(-1, -9, 2, -3, -1, 2),
                    new Day12.Moon(4, 1, 5, 2, 0, -2),
                    new Day12.Moon(2, 2, -4, 0, -1, 2),
                    new Day12.Moon(3, -7, -1, 1, 2, -2)
                });

            day12.TakeStep();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(-1, -7, 3, 0, 2, 1),
                    new Day12.Moon(3, 0, 0, -1, -1, -5),
                    new Day12.Moon(3, -2, 1, 1, -4, 5),
                    new Day12.Moon(3, -4, -2, 0, 3, -1)
                });

            day12.TakeStep();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(2, -2, 1, 3, 5, -2),
                    new Day12.Moon(1, -4, -4, -2, -4, -4),
                    new Day12.Moon(3, -7, 5, 0, -5, 4),
                    new Day12.Moon(2, 0, 0, -1, 4, 2)
                });

            day12.TakeStep();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(5, 2, -2, 3, 4, -3),
                    new Day12.Moon(2, -7, -5, 1, -3, -1),
                    new Day12.Moon(0, -9, 6, -3, -2, 1),
                    new Day12.Moon(1, 1, 3, -1, 1, 3)
                });

            day12.TakeStep();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(5, 3, -4, 0, 1, -2),
                    new Day12.Moon(2, -9, -3, 0, -2, 2),
                    new Day12.Moon(0, -8, 4, 0, 1, -2),
                    new Day12.Moon(1, 1, 5, 0, 0, 2)
                });

            day12.TakeStep();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(2, 1, -3, -3, -2, 1),
                    new Day12.Moon(1, -8, 0, -1, 1, 3),
                    new Day12.Moon(3, -6, 1, 3, 2, -3),
                    new Day12.Moon(2, 0, 4, 1, -1, -1)
                });

            day12.Moons[0].Energy.Should().Be(36);
            day12.Moons[1].Energy.Should().Be(45);
            day12.Moons[2].Energy.Should().Be(80);
            day12.Moons[3].Energy.Should().Be(18);

            day12.TotalEnergy.Should().Be(179);
        }

        [Test]
        public void Example2()
        {
            var input =
                "<x=-8, y=-10, z=0>\n<x=5, y=5, z=10>\n<x=2, y=-7, z=3>\n<x=9, y=-8, z=-3>"
                    .Split('\n');

            var day12 = new Day12(input);

            day12.TakeTenSteps();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(-9, -10, 1, -2, -2, -1),
                    new Day12.Moon(4, 10, 9, -3, 7, -2),
                    new Day12.Moon(8, -10, -3, 5, -1, -2),
                    new Day12.Moon(5, -10, 3, 0, -4, 5)
                });

            day12.TakeTenSteps();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(-10, 3, -4, -5, 2, 0),
                    new Day12.Moon(5, -25, 6, 1, 1, -4),
                    new Day12.Moon(13, 1, 1, 5, -2, 2),
                    new Day12.Moon(0, 1, 7, -1, -1, 2)
                });

            day12.TakeTenSteps();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(15, -6, -9, -5, 4, 0),
                    new Day12.Moon(-4, -11, 3, -3, -10, 0),
                    new Day12.Moon(0, -1, 11, 7, 4, 3),
                    new Day12.Moon(-3, -2, 5, 1, 2, -3)
                });

            day12.TakeTenSteps();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(14, -12, -4, 11, 3, 0),
                    new Day12.Moon(-1, 18, 8, -5, 2, 3),
                    new Day12.Moon(-5, -14, 8, 1, -2, 0),
                    new Day12.Moon(0, -12, -2, -7, -3, -3)
                });

            day12.TakeTenSteps();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(-23, 4, 1, -7, -1, 2),
                    new Day12.Moon(20, -31, 13, 5, 3, 4),
                    new Day12.Moon(-4, 6, 1, -1, 1, -3),
                    new Day12.Moon(15, 1, -5, 3, -3, -3)
                });

            day12.TakeTenSteps();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(36, -10, 6, 5, 0, 3),
                    new Day12.Moon(-18, 10, 9, -3, -7, 5),
                    new Day12.Moon(8, -12, -3, -2, 1, -7),
                    new Day12.Moon(-18, -8, -2, 0, 6, -1)
                });

            day12.TakeTenSteps();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(-33, -6, 5, -5, -4, 7),
                    new Day12.Moon(13, -9, 2, -2, 11, 3),
                    new Day12.Moon(11, -8, 2, 8, -6, -7),
                    new Day12.Moon(17, 3, 1, -1, -1, -3)
                });

            day12.TakeTenSteps();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(30, -8, 3, 3, 3, 0),
                    new Day12.Moon(-2, -4, 0, 4, -13, 2),
                    new Day12.Moon(-18, -7, 15, -8, 2, -2),
                    new Day12.Moon(-2, -1, -8, 1, 8, 0)
                });

            day12.TakeTenSteps();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(-25, -1, 4, 1, -3, 4),
                    new Day12.Moon(2, -9, 0, -3, 13, -1),
                    new Day12.Moon(32, -8, 14, 5, -4, 6),
                    new Day12.Moon(-1, -2, -8, -3, -6, -9)
                });

            day12.TakeTenSteps();
            day12.Moons.Should().BeEquivalentTo(
                new List<Day12.Moon>
                {
                    new Day12.Moon(8, -12, -9, -7, 3, 0),
                    new Day12.Moon(13, 16, -3, 3, -11, -5),
                    new Day12.Moon(-29, -11, -1, -3, 7, 4),
                    new Day12.Moon(16, -13, 23, 7, 1, 1)
                });

            day12.Moons[0].Energy.Should().Be(290);
            day12.Moons[1].Energy.Should().Be(608);
            day12.Moons[2].Energy.Should().Be(574);
            day12.Moons[3].Energy.Should().Be(468);

            day12.TotalEnergy.Should().Be(1940);
        }

        [Test]
        public void Example1_Repetition()
        {
            var input =
                "<x=-1, y=0, z=2>\n<x=2, y=-10, z=-7>\n<x=4, y=-8, z=8>\n<x=3, y=5, z=-1>"
                    .Split('\n');

            var day12 = new Day12(input);
            var count = day12.findRepetition();

            count.Should().Be(2772L);
        }

        [Test]
        public void Example2_Repetition()
        {
            var input =
                "<x=-8, y=-10, z=0>\n<x=5, y=5, z=10>\n<x=2, y=-7, z=3>\n<x=9, y=-8, z=-3>"
                    .Split('\n');

            var day12 = new Day12(input);
            var count = day12.findRepetition();

            count.Should().Be(4686774924L);
        }

        [Test]
        public void Part1()
        {
            var day12 = new Day12(input.linesOfStrings());
            day12.TakeSteps(1000);

            day12.TotalEnergy.Should().Be(10664);
        }

        [Test]
        public void Part2()
        {
            var day12 = new Day12(input.linesOfStrings());
            var count = day12.findRepetition();
            count.Should().Be(303459551979256L);
        }
    }
}
