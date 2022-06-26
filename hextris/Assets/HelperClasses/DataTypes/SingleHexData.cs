using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleHexData {

    public GridPosition position;
    public Color color;


    public SingleHexData(GridPosition position, Color color) {
        this.position = position;
        this.color = color;
    }

    public SingleHexData(Color color) {
        this.position = new GridPosition(0, 0);
        this.color = color;
    }

}