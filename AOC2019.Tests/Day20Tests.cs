using System.Drawing;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace AOC2019.Tests
{
    public class Day20Tests : TestBase
    {
        /*
         * --- Day 20: Donut Maze ---
         *
         * You notice a strange pattern on the surface of Pluto and land
         * nearby to get a closer look. Upon closer inspection, you
         * realize you've come across one of the famous space-warping
         * mazes of the long-lost Pluto civilization!
         *
         * Because there isn't much space on Pluto, the civilization
         * that used to live here thrived by inventing a method for
         * folding spacetime. Although the technology is no longer
         * understood, mazes like this one provide a small glimpse into
         * the daily life of an ancient Pluto citizen.
         *
         * This maze is shaped like a donut. Portals along the inner and
         * outer edge of the donut can instantly teleport you from one
         * side to the other. For example:
         *
         *          A
         *          A
         *   #######.#########
         *   #######.........#
         *   #######.#######.#
         *   #######.#######.#
         *   #######.#######.#
         *   #####  B    ###.#
         * BC...##  C    ###.#
         *   ##.##       ###.#
         *   ##...DE  F  ###.#
         *   #####    G  ###.#
         *   #########.#####.#
         * DE..#######...###.#
         *   #.#########.###.#
         * FG..#########.....#
         *   ###########.#####
         *              Z
         *              Z
         *
         * This map of the maze shows solid walls (#) and open passages
         * (.). Every maze on Pluto has a start (the open tile next to
         * AA) and an end (the open tile next to ZZ). Mazes on Pluto
         * also have portals; this maze has three pairs of portals: BC,
         * DE, and FG. When on an open tile next to one of these labels,
         * a single step can take you to the other tile with the same
         * label. (You can only walk on . tiles; labels and empty space
         * are not traversable.)
         *
         * One path through the maze doesn't require any portals.
         * Starting at AA, you could go down 1, right 8, down 12, left
         * 4, and down 1 to reach ZZ, a total of 26 steps.
         *
         * However, there is a shorter path: You could walk from AA to
         * the inner BC portal (4 steps), warp to the outer BC portal
         * (1 step), walk to the inner DE (6 steps), warp to the outer
         * DE (1 step), walk to the outer FG (4 steps), warp to the
         * inner FG (1 step), and finally walk to ZZ (6 steps). In
         * total, this is only 23 steps.
         *
         * Here is a larger example:
         *
         *                    A
         *                    A
         *   #################.#############
         *   #.#...#...................#.#.#
         *   #.#.#.###.###.###.#########.#.#
         *   #.#.#.......#...#.....#.#.#...#
         *   #.#########.###.#####.#.#.###.#
         *   #.............#.#.....#.......#
         *   ###.###########.###.#####.#.#.#
         *   #.....#        A   C    #.#.#.#
         *   #######        S   P    #####.#
         *   #.#...#                 #......VT
         *   #.#.#.#                 #.#####
         *   #...#.#               YN....#.#
         *   #.###.#                 #####.#
         * DI....#.#                 #.....#
         *   #####.#                 #.###.#
         * ZZ......#               QG....#..AS
         *   ###.###                 #######
         * JO..#.#.#                 #.....#
         *   #.#.#.#                 ###.#.#
         *   #...#..DI             BU....#..LF
         *   #####.#                 #.#####
         * YN......#               VT..#....QG
         *   #.###.#                 #.###.#
         *   #.#...#                 #.....#
         *   ###.###    J L     J    #.#.###
         *   #.....#    O F     P    #.#...#
         *   #.###.#####.#.#####.#####.###.#
         *   #...#.#.#...#.....#.....#.#...#
         *   #.#####.###.###.#.#.#########.#
         *   #...#.#.....#...#.#.#.#.....#.#
         *   #.###.#####.###.###.#.#.#######
         *   #.#.........#...#.............#
         *   #########.###.###.#############
         *            B   J   C
         *            U   P   P
         *
         * Here, AA has no direct path to ZZ, but it does connect to AS
         * and CP. By passing through AS, QG, BU, and JO, you can reach
         * ZZ in 58 steps.
         *
         * In your maze, how many steps does it take to get from the
         * open tile marked AA to the open tile marked ZZ?
         *
         *
         * --- Part Two ---
         *
         * Strangely, the exit isn't open when you reach it. Then, you
         * remember: the ancient Plutonians were famous for building
         * recursive spaces.
         *
         * The marked connections in the maze aren't portals: they
         * physically connect to a larger or smaller copy of the maze.
         * Specifically, the labeled tiles around the inside edge
         * actually connect to a smaller copy of the same maze, and the
         * smaller copy's inner labeled tiles connect to yet a smaller
         * copy, and so on.
         *
         * When you enter the maze, you are at the outermost level; when
         * at the outermost level, only the outer labels AA and ZZ
         * function (as the start and end, respectively); all other
         * outer labeled tiles are effectively walls. At any other level,
         * AA and ZZ count as walls, but the other outer labeled tiles
         * bring you one level outward.
         *
         * Your goal is to find a path through the maze that brings you
         * back to ZZ at the outermost level of the maze.
         *
         * In the first example above, the shortest path is now the loop
         * around the right side. If the starting level is 0, then
         * taking the previously-shortest path would pass through BC (to
         * level 1), DE (to level 2), and FG (back to level 1). Because
         * this is not the outermost level, ZZ is a wall, and the only
         * option is to go back around to BC, which would only send you
         * even deeper into the recursive maze.
         *
         * In the second example above, there is no path that brings you
         * to ZZ at the outermost level.
         *
         * Here is a more interesting example:
         *
         *              Z L X W       C
         *              Z P Q B       K
         *   ###########.#.#.#.#######.###############
         *   #...#.......#.#.......#.#.......#.#.#...#
         *   ###.#.#.#.#.#.#.#.###.#.#.#######.#.#.###
         *   #.#...#.#.#...#.#.#...#...#...#.#.......#
         *   #.###.#######.###.###.#.###.###.#.#######
         *   #...#.......#.#...#...#.............#...#
         *   #.#########.#######.#.#######.#######.###
         *   #...#.#    F       R I       Z    #.#.#.#
         *   #.###.#    D       E C       H    #.#.#.#
         *   #.#...#                           #...#.#
         *   #.###.#                           #.###.#
         *   #.#....OA                       WB..#.#..ZH
         *   #.###.#                           #.#.#.#
         * CJ......#                           #.....#
         *   #######                           #######
         *   #.#....CK                         #......IC
         *   #.###.#                           #.###.#
         *   #.....#                           #...#.#
         *   ###.###                           #.#.#.#
         * XF....#.#                         RF..#.#.#
         *   #####.#                           #######
         *   #......CJ                       NM..#...#
         *   ###.#.#                           #.###.#
         * RE....#.#                           #......RF
         *   ###.###        X   X       L      #.#.#.#
         *   #.....#        F   Q       P      #.#.#.#
         *   ###.###########.###.#######.#########.###
         *   #.....#...#.....#.......#...#.....#.#...#
         *   #####.#.###.#######.#######.###.###.#.#.#
         *   #.......#.......#.#.#.#.#...#...#...#.#.#
         *   #####.###.#####.#.#.#.#.###.###.#.###.###
         *   #.......#.....#.#...#...............#...#
         *   #############.#.#.###.###################
         *                A O F   N
         *                A A D   M
         * One shortest path through the maze is the following:
         *
         * Walk from AA to XF (16 steps)
         * Recurse into level 1 through XF (1 step)
         * Walk from XF to CK (10 steps)
         * Recurse into level 2 through CK (1 step)
         * Walk from CK to ZH (14 steps)
         * Recurse into level 3 through ZH (1 step)
         * Walk from ZH to WB (10 steps)
         * Recurse into level 4 through WB (1 step)
         * Walk from WB to IC (10 steps)
         * Recurse into level 5 through IC (1 step)
         * Walk from IC to RF (10 steps)
         * Recurse into level 6 through RF (1 step)
         * Walk from RF to NM (8 steps)
         * Recurse into level 7 through NM (1 step)
         * Walk from NM to LP (12 steps)
         * Recurse into level 8 through LP (1 step)
         * Walk from LP to FD (24 steps)
         * Recurse into level 9 through FD (1 step)
         * Walk from FD to XQ (8 steps)
         * Recurse into level 10 through XQ (1 step)
         * Walk from XQ to WB (4 steps)
         * Return to level 9 through WB (1 step)
         * Walk from WB to ZH (10 steps)
         * Return to level 8 through ZH (1 step)
         * Walk from ZH to CK (14 steps)
         * Return to level 7 through CK (1 step)
         * Walk from CK to XF (10 steps)
         * Return to level 6 through XF (1 step)
         * Walk from XF to OA (14 steps)
         * Return to level 5 through OA (1 step)
         * Walk from OA to CJ (8 steps)
         * Return to level 4 through CJ (1 step)
         * Walk from CJ to RE (8 steps)
         * Return to level 3 through RE (1 step)
         * Walk from RE to IC (4 steps)
         * Recurse into level 4 through IC (1 step)
         * Walk from IC to RF (10 steps)
         * Recurse into level 5 through RF (1 step)
         * Walk from RF to NM (8 steps)
         * Recurse into level 6 through NM (1 step)
         * Walk from NM to LP (12 steps)
         * Recurse into level 7 through LP (1 step)
         * Walk from LP to FD (24 steps)
         * Recurse into level 8 through FD (1 step)
         * Walk from FD to XQ (8 steps)
         * Recurse into level 9 through XQ (1 step)
         * Walk from XQ to WB (4 steps)
         * Return to level 8 through WB (1 step)
         * Walk from WB to ZH (10 steps)
         * Return to level 7 through ZH (1 step)
         * Walk from ZH to CK (14 steps)
         * Return to level 6 through CK (1 step)
         * Walk from CK to XF (10 steps)
         * Return to level 5 through XF (1 step)
         * Walk from XF to OA (14 steps)
         * Return to level 4 through OA (1 step)
         * Walk from OA to CJ (8 steps)
         * Return to level 3 through CJ (1 step)
         * Walk from CJ to RE (8 steps)
         * Return to level 2 through RE (1 step)
         * Walk from RE to XQ (14 steps)
         * Return to level 1 through XQ (1 step)
         * Walk from XQ to FD (8 steps)
         * Return to level 0 through FD (1 step)
         * Walk from FD to ZZ (18 steps)
         * This path takes a total of 396 steps to move from AA at the outermost layer to ZZ at the outermost layer.
         *
         * In your maze, when accounting for recursion, how many steps does it take to get from the open tile marked AA to the open tile marked ZZ, both at the outermost layer?
         *
         */
        public Day20Tests() : base(20) { }

        [Test]
        public void MapParsingTests()
        {
          var map = @"         A           
         A           
  #######.#########  
  #######.........#  
  #######.#######.#  
  #######.#######.#  
  #######.#######.#  
  #####  B    ###.#  
BC...##  C    ###.#  
  ##.##       ###.#  
  ##...DE  F  ###.#  
  #####    G  ###.#  
  #########.#####.#  
DE..#######...###.#  
  #.#########.###.#  
FG..#########.....#  
  ###########.#####  
             Z       
             Z       ".Split("\r\n").ToList();

          var maze = new Day20.Maze(map);

          maze.Entrance.Should().BeEquivalentTo(new Point(7, 0));
          maze.Exit.Should().BeEquivalentTo(new Point(11, 14));

          maze.OuterPortalNames.Should().HaveCount(3);
          maze.OuterPortalNames.Should().ContainKey("BC");
          maze.OuterPortalNames.Should().ContainKey("DE");
          maze.OuterPortalNames.Should().ContainKey("FG");
          maze.OuterPortalNames["BC"].Should().BeEquivalentTo(new Point(0, 6));
          maze.OuterPortalNames["DE"].Should().BeEquivalentTo(new Point(0, 11));
          maze.OuterPortalNames["FG"].Should().BeEquivalentTo(new Point(0, 13));

          maze.InnerPortalNames.Should().ContainKey("BC");
          maze.InnerPortalNames.Should().ContainKey("DE");
          maze.InnerPortalNames.Should().ContainKey("FG");
          maze.InnerPortalNames["BC"].Should().BeEquivalentTo(new Point(7, 4));
          maze.InnerPortalNames["DE"].Should().BeEquivalentTo(new Point(4, 8));
          maze.InnerPortalNames["FG"].Should().BeEquivalentTo(new Point(9, 10));

          maze.Portals.Should().ContainKey(new Point(0, 6));

          maze.Paths.Should().HaveCount(7);
          maze.Paths.Should().ContainKey(new Point(7, 0));
          maze.Paths.Should().ContainKey(new Point(0, 6));
          maze.Paths.Should().ContainKey(new Point(0, 11));
          maze.Paths.Should().ContainKey(new Point(0, 13));
          maze.Paths.Should().ContainKey(new Point(7, 4));
          maze.Paths.Should().ContainKey(new Point(4, 8));
          maze.Paths.Should().ContainKey(new Point(9, 10));

          maze.Paths[new Point(7, 0)].Should().HaveCount(3);
          maze.Paths[new Point(7, 0)].Should().ContainEquivalentOf(new Day20.Path(new Point(7, 4), 4));
          maze.Paths[new Point(7, 0)].Should().ContainEquivalentOf(new Day20.Path(new Point(9, 10), 30));
          maze.Paths[new Point(7, 0)].Should().ContainEquivalentOf(new Day20.Path(new Point(11, 14), 26));

          maze.Paths[new Point(7, 4)].Should().HaveCount(2);
          maze.Paths[new Point(7, 4)].Should().ContainEquivalentOf(new Day20.Path(new Point(9, 10), 32));
          maze.Paths[new Point(7, 4)].Should().ContainEquivalentOf(new Day20.Path(new Point(11, 14), 28));

          maze.Paths[new Point(9, 10)].Should().HaveCount(2);
          maze.Paths[new Point(9, 10)].Should().ContainEquivalentOf(new Day20.Path(new Point(7, 4), 32));
          maze.Paths[new Point(9, 10)].Should().ContainEquivalentOf(new Day20.Path(new Point(11, 14), 6));

          maze.Paths[new Point(0, 6)].Should().HaveCount(1);
          maze.Paths[new Point(0, 6)].Should().ContainEquivalentOf(new Day20.Path(new Point(4, 8), 6));
          maze.Paths[new Point(4, 8)].Should().HaveCount(1);
          maze.Paths[new Point(4, 8)].Should().ContainEquivalentOf(new Day20.Path(new Point(0, 6), 6));

          maze.Paths[new Point(0, 11)].Should().HaveCount(1);
          maze.Paths[new Point(0, 11)].Should().ContainEquivalentOf(new Day20.Path(new Point(0, 13), 4));
          maze.Paths[new Point(0, 13)].Should().HaveCount(1);
          maze.Paths[new Point(0, 13)].Should().ContainEquivalentOf(new Day20.Path(new Point(0, 11), 4));
        }

        [Test]
        public void MapNavigationTest()
        {
            var map = @"         A           
         A           
  #######.#########  
  #######.........#  
  #######.#######.#  
  #######.#######.#  
  #######.#######.#  
  #####  B    ###.#  
BC...##  C    ###.#  
  ##.##       ###.#  
  ##...DE  F  ###.#  
  #####    G  ###.#  
  #########.#####.#  
DE..#######...###.#  
  #.#########.###.#  
FG..#########.....#  
  ###########.#####  
             Z       
             Z       ".Split("\r\n").ToList();

            var day20 = new Day20(map);
            day20.ShortestRoute().Should().Be(23);
        }

        [Test]
        public void BiggerMapNavigationTest()
        {
            var map = @"                   A               
                   A               
  #################.#############  
  #.#...#...................#.#.#  
  #.#.#.###.###.###.#########.#.#  
  #.#.#.......#...#.....#.#.#...#  
  #.#########.###.#####.#.#.###.#  
  #.............#.#.....#.......#  
  ###.###########.###.#####.#.#.#  
  #.....#        A   C    #.#.#.#  
  #######        S   P    #####.#  
  #.#...#                 #......VT
  #.#.#.#                 #.#####  
  #...#.#               YN....#.#  
  #.###.#                 #####.#  
DI....#.#                 #.....#  
  #####.#                 #.###.#  
ZZ......#               QG....#..AS
  ###.###                 #######  
JO..#.#.#                 #.....#  
  #.#.#.#                 ###.#.#  
  #...#..DI             BU....#..LF
  #####.#                 #.#####  
YN......#               VT..#....QG
  #.###.#                 #.###.#  
  #.#...#                 #.....#  
  ###.###    J L     J    #.#.###  
  #.....#    O F     P    #.#...#  
  #.###.#####.#.#####.#####.###.#  
  #...#.#.#...#.....#.....#.#...#  
  #.#####.###.###.#.#.#########.#  
  #...#.#.....#...#.#.#.#.....#.#  
  #.###.#####.###.###.#.#.#######  
  #.#.........#...#.............#  
  #########.###.###.#############  
           B   J   C               
           U   P   P               ".Split("\r\n").ToList();

            var day20 = new Day20(map);
            day20.ShortestRoute().Should().Be(58);
        }

        [Test]
        public void RecursiveMapNavigationTest()
        {
          var map = @"         A           
         A           
  #######.#########  
  #######.........#  
  #######.#######.#  
  #######.#######.#  
  #######.#######.#  
  #####  B    ###.#  
BC...##  C    ###.#  
  ##.##       ###.#  
  ##...DE  F  ###.#  
  #####    G  ###.#  
  #########.#####.#  
DE..#######...###.#  
  #.#########.###.#  
FG..#########.....#  
  ###########.#####  
             Z       
             Z       ".Split("\r\n").ToList();

          var day20 = new Day20(map);
          day20.ShortestRoute(true).Should().Be(26);
        }

        [Test]
        public void BiggerRecursiveMapNavigationTest()
        {
          var map = @"             Z L X W       C                 
             Z P Q B       K                 
  ###########.#.#.#.#######.###############  
  #...#.......#.#.......#.#.......#.#.#...#  
  ###.#.#.#.#.#.#.#.###.#.#.#######.#.#.###  
  #.#...#.#.#...#.#.#...#...#...#.#.......#  
  #.###.#######.###.###.#.###.###.#.#######  
  #...#.......#.#...#...#.............#...#  
  #.#########.#######.#.#######.#######.###  
  #...#.#    F       R I       Z    #.#.#.#  
  #.###.#    D       E C       H    #.#.#.#  
  #.#...#                           #...#.#  
  #.###.#                           #.###.#  
  #.#....OA                       WB..#.#..ZH
  #.###.#                           #.#.#.#  
CJ......#                           #.....#  
  #######                           #######  
  #.#....CK                         #......IC
  #.###.#                           #.###.#  
  #.....#                           #...#.#  
  ###.###                           #.#.#.#  
XF....#.#                         RF..#.#.#  
  #####.#                           #######  
  #......CJ                       NM..#...#  
  ###.#.#                           #.###.#  
RE....#.#                           #......RF
  ###.###        X   X       L      #.#.#.#  
  #.....#        F   Q       P      #.#.#.#  
  ###.###########.###.#######.#########.###  
  #.....#...#.....#.......#...#.....#.#...#  
  #####.#.###.#######.#######.###.###.#.#.#  
  #.......#.......#.#.#.#.#...#...#...#.#.#  
  #####.###.#####.#.#.#.#.###.###.#.###.###  
  #.......#.....#.#...#...............#...#  
  #############.#.#.###.###################  
               A O F   N                     
               A A D   M                     ".Split("\r\n").ToList();

          var day20 = new Day20(map);
          day20.ShortestRoute(true).Should().Be(396);
        }

        [Test]
        public void Part1()
        {
          var day20 = new Day20(input.linesOfStrings());
          day20.ShortestRoute().Should().Be(484);
        }

        [Test]
        public void Part2()
        {
          var day20 = new Day20(input.linesOfStrings());
          day20.ShortestRoute(true).Should().Be(5754);
        }

    }
}
