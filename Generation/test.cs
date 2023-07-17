using Generation;

int x, y;
x = 40;
y = 40;

BuildingsGenerator bg = new(x, y, 5, (int)DateTime.UtcNow.Ticks);
try { bg.placeBases(); }
catch (ArgumentException)
{
    try
    {
        bg.placeBases();
    }
    catch
    {
        while (true)
        {
            Console.WriteLine("AHAHHAHAHHAHAHAHHAHAHAHHAHAHAHHA");
        }
    }//ну если с 2 раза не сгенерит то это судьба епта
}

bg.placeNecessaryFlags();
bg.placeAdditionlFlagsNearPoint(5, bg.middle);
bg.generateInterestPoints(bg.middle);
bg.generateMatrix();
RoadGenerator roadGenerator = new RoadGenerator(bg.placedBases, bg.getUnweightedFlags(), bg.interestPoints, x, y, bg.matrix);
roadGenerator.connectAllObjectives();
Console.WriteLine(roadGenerator);