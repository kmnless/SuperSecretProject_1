using Generation;

int x, y;
x = 40;
y = 40;

BuildingsGenerator bg = new(x, y, 5, (int)DateTime.UtcNow.Ticks);
bg.placeBases();
bg.placeNecessaryFlags();
bg.generateMatrix();

RoadGenerator roadGenerator = new RoadGenerator(bg.placedBases, bg.placedFlags, x, y, bg.matrix);
roadGenerator.connectAllObjectives();
Console.WriteLine(roadGenerator);