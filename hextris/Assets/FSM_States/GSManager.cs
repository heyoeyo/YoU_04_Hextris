using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSManager : MonoBehaviour {

    // Special data set used to initialize grid data (for debugging)
    [SerializeField] float start_delay_sec = 1f;
    [SerializeField] HexListSO debug_grid_init;

    // Data sets used to assign hexes to different display/animation states
    public GridData grid;
    public HexListSO player_hexes;
    public HexListSO landing_hexes;
    public HexListSO queue_hexes;
    public HexListSO removal_hexes;
    public HexListSO dropped_left_hexes;
    public HexListSO dropped_right_hexes;

    // State-specific data (should be held by states directly...)
    [SerializeField] QueueSO shape_queue;
    [SerializeField] IntSO game_level;

    // Bookkeeping variables that need to persist/be shared through states
    [HideInInspector] public MovableHexGroup player_group;
    [HideInInspector] public MovableHexGroup landing_group;
    [HideInInspector] public Stack<int> cleared_row_idxs;

    // Set up each of the game states
    GameState state;
    InitGameState _init_state;
    SpawnGameState _spawn_state;
    FallGameState _fall_state;
    FullFallGameState _full_fall_state;
    LockGameState _lock_state;
    ClearRowsGameState _clear_rows_state;
    DropCellsGameState _drop_cells_state;
    StuckGameState _stuck_state;


    // ----------------------------------------------------------------------------------------------------------------
    // Built-ins

    private void Start() {

        // Make sure we reset 'global' data storage when game starts
        grid.Init();
        shape_queue.Init();

        // Initialize grid if needed
        if (debug_grid_init != null) grid.SetStartState(debug_grid_init.init_mask);

        // Instantiate all states
        this._init_state = new(this, this.start_delay_sec);
        this._spawn_state = new SpawnGameState(this, this.shape_queue);
        this._fall_state = new FallGameState(this, this.game_level);
        this._full_fall_state = new FullFallGameState(this);
        this._lock_state = new LockGameState(this);
        this._clear_rows_state = new ClearRowsGameState(this);
        this._drop_cells_state = new DropCellsGameState(this);
        this._stuck_state = new StuckGameState(this);

        InitState();
    }

    private void Update() {
        state.Update();
    }


    // ----------------------------------------------------------------------------------------------------------------
    // State transistion functions

    private void InitState() => EnterState(this._init_state);
    public void EnterSpawn() => EnterState(this._spawn_state);
    public void EnterFall() => EnterState(this._fall_state);
    public void EnterFullFall() => EnterState(this._full_fall_state);
    public void EnterLock() => EnterState(this._lock_state);
    public void EnterClearRows() => EnterState(this._clear_rows_state);
    public void EnterDropCells() => EnterState(this._drop_cells_state);
    public void EnterStuck() => EnterState(this._stuck_state);

    private void EnterState(GameState new_state) {

        //Debug.Log("ENTERSTATE: " + new_state.GetType().Name);

        if (this.state != null) this.state.Exit();
        this.state = new_state;
        this.state.Enter();
    }

}