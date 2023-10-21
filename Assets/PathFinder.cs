using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Coordinate {
  public int x;
  public int y;
  public Coordinate(int i, int j) {
    x = i;
    y = j;
  }
}

public enum FieldStates {
  Finish,
  Start,
  Wall,
  Empty,
  Discovered,
  Known,
  Solution
}
public class smallestCostArray {

  List<Node> queue;
  public smallestCostArray() { this.queue = new List<Node>(); }
  public bool addElement(Node candidate) {
    Coordinate coordinate = candidate.getPos();
    bool trigered = false;
    foreach (Node resident in queue) {
      if (resident.getPos() == coordinate) {
        trigered = true;
        if (resident.getFCost() == candidate.getFCost() &&
                resident.getHCost() > candidate.getHCost() ||
            resident.getFCost() > candidate.getFCost()) 
        {
          
          queue.Remove(resident);
          queue.Add(candidate);
          return true;
        }
      }
    }
    if (!trigered) {
      queue.Add(candidate);
      return true;
    }
    return false;
  }
  public Node pollLowest() {
    if (queue.Count == 0) {
      return null;
    }
    Node lowest = queue[queue.Count - 1];
    foreach (Node resident in queue) {
      if (lowest.getFCost() > resident.getFCost() ||
          (resident.getFCost() == lowest.getFCost() &&
           lowest.getHCost() > resident.getHCost())) {
        lowest = resident;
      }
    }
    queue.Remove(lowest);
    return lowest;
  }

  public Node peekLowest() {
    if (queue.Count == 0) {
      return null;
    }
    Node lowest = queue[queue.Count - 1];
    foreach (Node resident in queue) {
      if (lowest.getFCost() > resident.getFCost() ||
          (resident.getFCost() == lowest.getFCost() &&
           lowest.getHCost() > resident.getHCost())) {
        lowest = resident;
      }
    }
    queue.Remove(lowest);
    return lowest;
  }
  public int size() { return queue.Count; }
}
public class Node {
  private Coordinate father;
  private int GCost;
  private int HCost;
  private int FCost;
  private FieldStates state;
  private Coordinate pos;
  public Node(Coordinate father, int cost, FieldStates state,
              Coordinate pos) {
    this.father = father;
    this.GCost = cost;
    this.HCost = cost;
    this.FCost = cost;
    this.state = state;
    this.pos = pos;
  }
  public void ChangeState(FieldStates state) { this.state = state; }
  public void ChangeFather(Coordinate father) { this.father = father; }
  public void ChangeCosts(int G, int H) {
    this.GCost = G;
    this.HCost = H;
    this.FCost = this.GCost + this.HCost;
  }
  public FieldStates getState() { return this.state; }
  public int getFCost() { return FCost; }
  public int getGCost() { return GCost; }
  public int getHCost() { return HCost; }
  public Coordinate getPos() { return pos; }
  public Coordinate getFather() { return father; }
}

public class AStar {
  private Node[,] field;
  private int actionCost = 10;
  private smallestCostArray candidates;
  private Coordinate start, finish;
  private HashSet<Coordinate> searched;
  private int sizeX, sizeY;
  private bool going = true;
  private bool end_ = false;
  public AStar(FieldStates[,] field){
    int i, j;
    if (field == null || field.GetLength(0) == 0) {
      throw new Exception("Field was bad");
    }
    sizeX = field.GetLength(0);
    sizeY = field.GetLength(1);
    this.field = new Node[sizeX,sizeY];
    for (i = 0; i < sizeX; ++i) {
      for (j = 0; j < sizeY; ++j) {
        this.field[i,j] = new Node(null, 0, field[i,j], new Coordinate(i, j));
        if (field[i,j] == FieldStates.Start) {
          start = this.field[i,j].getPos();
        } else if (field[i,j] == FieldStates.Finish) {
          finish = this.field[i,j].getPos();
        }
      }
    }
    searched = new HashSet<Coordinate>();
    if (start == null || finish == null) {
      throw new Exception("No start or finish");
    }
    this.field[start.x,start.y].ChangeState(FieldStates.Start);
    this.field[finish.x,finish.y].ChangeState(FieldStates.Finish);
    this.candidates = new smallestCostArray();
  }

  public FieldStates[,] getField() {
    FieldStates[,] exhaust = new FieldStates[sizeX,sizeY];
    for (int i = 0; i < sizeX; i++) {
      for (int j = 0; j < sizeY; j++) {
        exhaust[i,j] = this.field[i,j].getState();
      }
    }
    return exhaust;
  }

  private void getPossibilities(Coordinate pos) {
    int i, j, GCost, HCost;
    Coordinate candidate;
    Node candidateNode;
    for (i = (pos.x - 1); i < pos.x + 2; ++i) {
      for (j = (pos.y - 1); j < pos.y + 2; ++j) {
        if (i >= 0 && i < sizeX && j >= 0 && j < sizeY &&
            field[i,j].getState() != FieldStates.Wall) {
          if (i != pos.x || j != pos.y) {
            candidate = field[i,j].getPos();
            GCost = (int)(Math.Sqrt(Math.Abs(i - pos.x) + Math.Abs(j - pos.y)) *
                          actionCost) +
                    field[pos.x,pos.y].getGCost();
            HCost = calculateHypothetical(candidate);
            if (!searched.Contains(candidate)) {
              candidateNode =
                  new Node(pos, GCost + HCost, FieldStates.Known, candidate);
              candidateNode.ChangeCosts(GCost, HCost);
              if (field[i,j].getState() != FieldStates.Finish) {
                field[i,j].ChangeState(FieldStates.Known);
              }
              candidates.addElement(candidateNode);
            } else {
              if (field[candidate.x,candidate.y].getGCost() >= GCost) {
                field[candidate.x,candidate.y].ChangeCosts(GCost, HCost);
                field[candidate.x,candidate.y].ChangeFather(pos);
              }
            }
          }
        }
      }
    }
  }
  private Coordinate makeAnOperation() {
    if (candidates.size() == 0) {
      return null;
    }
    Node candidate = candidates.pollLowest();
    field[candidate.getPos().x,candidate.getPos().y].ChangeCosts(
        candidate.getGCost(), candidate.getHCost());
    field[candidate.getPos().x,candidate.getPos().y].ChangeCosts(
        candidate.getGCost(), candidate.getHCost());
    field[candidate.getPos().x,candidate.getPos().y].ChangeFather(
        candidate.getFather());
    if (field[candidate.getPos().x,candidate.getPos().y].getState() !=
        FieldStates.Finish) {
      field[candidate.getPos().x,candidate.getPos().y].ChangeState(
          FieldStates.Discovered);
    }
    searched.Add(candidate.getPos());
    return candidate.getPos();
  }

  private int calculateHypothetical(Coordinate point) {
    return (Math.Abs(finish.x - point.x) + Math.Abs(finish.y - point.y)) *
        actionCost;
  }
  private List<Coordinate> getPath(Coordinate end) {
    List<Coordinate> path = new List<Coordinate>();
    Coordinate pos = end;
    while (pos != start) {
      path.Add(pos);
      pos = field[pos.x,pos.y].getFather();
      if (pos != start) {
        field[pos.x,pos.y].ChangeState(FieldStates.Solution);
      }
    }
    path.Add(start);
    return path;
  }
  public void forceStop() { going = false; }

  public List<Coordinate> solve(){
    Coordinate buffer;
    startSolving(0);
    while (going) {
      buffer = makeAnOperation();
      if (buffer == null) {
        end_ = true;
        throw new Exception("No solution");
      } else if (buffer.x == finish.x && buffer.y == finish.y) {
        end_ = true;
        return getPath(buffer);
      }

      getPossibilities(buffer);
    }
    return null;
  }
  public bool end() { return end_; }
  public void startSolving(int delay){

    field[start.x,start.y].ChangeCosts(0, calculateHypothetical(start));
    searched.Add(field[start.x,start.y].getPos());
    getPossibilities(start);
  }

  public int allowMove(){
    Coordinate buffer;
    buffer = makeAnOperation();
    // debug();
    // System.in.read();
    if (buffer == null) {
      throw new Exception("No solution");
    } else if (buffer.x == finish.x && buffer.y == finish.y) {
      getPath(finish);
      return 1;
    }

    getPossibilities(buffer);
    return 0;
  }

  
}