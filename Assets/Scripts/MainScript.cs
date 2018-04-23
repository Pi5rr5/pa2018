using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

/// <summary>
/// Classe principale � utiliser pour impl�menter vos algorithmes
/// Si vous souhaitez utiliser plusieurs scripts (1 par algorithme), 
/// vous le pouvez aussi.
/// </summary>
public class MainScript : MonoBehaviour
{

    /// <summary>
    /// Indique si un algorithme est en cours d'ex�cution
    /// </summary>
    private bool _isRunning = false;

    /// <summary>
    /// Indique si une evaluation de solution est en cours
    /// </summary>
    private bool _inSimulation = false;

    /// <summary>
    /// M�thode utilis�e pour g�rer les informations et 
    /// boutons de l'interface utilisateur
    /// </summary>
    public void OnGUI()
    {
        // D�marrage d'une liste de composants visuels verticale
        GUILayout.BeginVertical();

        // Affiche un bouton permettant le lancement de la recherche locale na�ve
        if (GUILayout.Button("DEMARRAGE RECHERCHE LOCALE NAIVE"))
        {
            // Le bouton est inactif si un algorithme est en cours d'ex�cution
            if (!_isRunning)
            {
                // D�marrage de la recherche locale na�ve en pseudo asynchrone
                StartCoroutine(nameof(NaiveLocalSearch));
            }
        }

        // Affiche un bouton permettant le lancement de la recherche locale na�ve
        if (GUILayout.Button("DEMARRAGE RECUIT SIMULE"))
        {
            // Le bouton est inactif si un algorithme est en cours d'ex�cution
            if (!_isRunning)
            {
                // D�marrage du recuit simul� en pseudo asynchrone
                StartCoroutine(nameof(SimulatedAnnealing));
            }
        }

        // Affiche un bouton permettant le lancement de l'algorithme g�n�tique
        if (GUILayout.Button("DEMARRAGE ALGORITHME GENETIQUE"))
        {
            // Le bouton est inactif si un algorithme est en cours d'ex�cution
            if (!_isRunning)
            {
                // D�marrage de l'algorithme g�n�tique en pseudo asynchrone
                StartCoroutine(nameof(GeneticAlgorithm));
            }
        }

        // Affiche un bouton permettant le lancement de l'algorithme de Djikstra
        if (GUILayout.Button("DEMARRAGE DJIKSTRA"))
        {
            // Le bouton est inactif si un algorithme est en cours d'ex�cution
            if (!_isRunning)
            {
                // D�marrage de l'algorithme de Djikstra en pseudo asynchrone
                StartCoroutine(nameof(Djikstra));
            }
        }

        // Affiche un bouton permettant le lancement de l'algorithme A*
        if (GUILayout.Button("DEMARRAGE A*"))
        {
            // Le bouton est inactif si un algorithme est en cours d'ex�cution
            if (!_isRunning)
            {
                // D�marrage de l'algorithme A* en pseudo asynchrone
                StartCoroutine(nameof(AStar));
            }
        }

        // Fin de la liste de composants visuels verticale
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Initialisation du script
    /// </summary>
    void Start()
    {
        // Pour faire en sorte que l'algorithme puisse continuer d'�tre actif m�me
        // en t�che de fond.
        Application.runInBackground = true;
    }

    /// <summary>
    /// Impl�mentation possible de la recherche locale na�ve
    /// sous forme de coroutine pour le mode pseudo asynchone
    /// </summary>
    /// <returns></returns>
    public IEnumerator NaiveLocalSearch()
    {
        // Indique que l'algorithme est en cours d'ex�cution
        _isRunning = true;

        // G�n�re une solution initiale au hazard (ici une s�quence
        // de 42 mouvements)
        var currentSolution = new PathSolutionScript(42);

        // R�cup�re le score de la solution initiale
        // Sachant que l'�valuation peut n�cessiter une 
        // simulation, pour pouvoir la visualiser nous
        // avons recours � une coroutine
        var scoreEnumerator = GetError(currentSolution);
        yield return StartCoroutine(scoreEnumerator);
        float currentError = scoreEnumerator.Current;

        // Nous r�cup�rons l'erreur minimum atteignable
        // Ceci est optionnel et d�pendant de la fonction
        // d'erreur
        var minimumError = GetMinError();

        // Affichage de l'erreur initiale
        Debug.Log(currentError);

        // Initialisation du nombre d'it�rations
        int iterations = 0;

        // Tout pendant que l'erreur minimale n'est pas atteinte
        while (currentError != minimumError)
        {
            // On obtient une copie de la solution courante
            // pour ne pas la modifier dans le cas ou la modification
            // ne soit pas conserv�e.
            var newsolution = CopySolution(currentSolution);

            // On proc�de � une petite modification de la solution
            // courante.
            RandomChangeInSolution(newsolution);

            // R�cup�re le score de la nouvelle solution
            // Sachant que l'�valuation peut n�cessiter une 
            // simulation, pour pouvoir la visualiser nous
            // avons recours � une coroutine
            var newscoreEnumerator = GetError(newsolution);
            yield return StartCoroutine(newscoreEnumerator);
            float newError = newscoreEnumerator.Current;

            // On affiche pour des raisons de Debug et de suivi
            // la comparaison entre l'erreur courante et la
            // nouvelle erreur
            Debug.Log(currentError + "   -   " + newError);

            // Si la solution a �t� am�lior�e
            if (newError <= currentError)
            {
                // On met � jour la solution courante
                currentSolution = newsolution;

                // On met � jour l'erreur courante
                currentError = newError;
            }

            // On incr�mente le nombre d'it�rations
            iterations++;

            // On rend la main au moteur Unity3D
            yield return 0;
        }

        // Fin de l'algorithme, on indique que son ex�cution est stopp�e
        _isRunning = false;

        // On affiche le nombre d'it�rations n�cessaire � l'algorithme pour trouver la solution
        Debug.Log("CONGRATULATIONS !!! Solution Found in " + iterations + " iterations !");
    }
    
    public struct currentNode  
    {  
        public int XPos;  
        public int YPos;  
    }  

    public IEnumerator Djikstra()
    {
        // Recovering the environment as a matrix
        var matrix = MatrixFromRaycast.CreateMatrixFromRayCast();

        /*
        // Convert as boolean grid
        // true => wall || explored
        // false => unknow
        */
        bool[][] booleanGrid = new bool[matrix.Length][];
        for (int i = 0; i < matrix.Length; i++)
        {
            booleanGrid[i] = new bool[matrix[i].Length];
            for (int j = 0; j < matrix[i].Length; j++)
            {
                booleanGrid[i][j] = (matrix[i][j] == LayerMask.NameToLayer("Obstacle")) ? true : false;
            }
        }
        
        //Label all the nodes with an infinite score
        int[][] Grid = new int[matrix.Length][];
        for (int i = 0; i < matrix.Length; i++)
        {
            Grid[i] = new int[matrix[i].Length];
            for (int j = 0; j < matrix[i].Length; j++)
            {
                if (matrix[i][j] == LayerMask.NameToLayer("Obstacle"))
                {
                    Grid[i][j] = int.MinValue;
                }
                else
                {
                    Grid[i][j] = int.MaxValue;
                }
            }
        }

        // get position
        var startPosX = PlayerScript.StartXPositionInMatrix;
        var startPosY = PlayerScript.StartYPositionInMatrix;
        var endPosX = PlayerScript.GoalXPositionInMatrix;
        var endPosY = PlayerScript.GoalYPositionInMatrix;
        
        // init start pos at 0
        Grid[startPosX][startPosY] = 0;
        booleanGrid[startPosX][startPosY] = true;
        
        //define move cost
        const int moveCost = 1;
        
        //define the current case
        var currentPos = new currentNode
        {
            XPos = startPosX,
            YPos = startPosY
        };

        var it = 0;
        while (true)
        {
            if (currentPos.XPos == endPosX && currentPos.YPos == endPosY)
            {
                Debug.Log("Solution found in "+it+" iterations");
                //todo return the optimal path
                break;
            }
            MarkNeighbourNode(Grid, booleanGrid, currentPos, moveCost);
            currentPos = MoveToNextNode(Grid, booleanGrid, currentPos, moveCost);
            it++;
        }

        yield return null;
    }

    //find and mark Neighbour Node if != Obstacle
    private static void MarkNeighbourNode(int[][] grid, bool[][] exploredGrid, currentNode currentPos, int moveCost)
    {
        var nextCost = grid[currentPos.XPos][currentPos.YPos] + moveCost;
        if (currentPos.XPos + 1 < grid.Length && !exploredGrid[currentPos.XPos + 1][currentPos.YPos] && grid[currentPos.XPos + 1][currentPos.YPos] == int.MaxValue)
            grid[currentPos.XPos + 1][currentPos.YPos] = nextCost;
        
        if (currentPos.XPos - 1 >= 0 && !exploredGrid[currentPos.XPos - 1][currentPos.YPos] && grid[currentPos.XPos - 1][currentPos.YPos] == int.MaxValue)
            grid[currentPos.XPos -1 ][currentPos.YPos] = nextCost;
        
        if (currentPos.YPos + 1 < grid[currentPos.XPos].Length && !exploredGrid[currentPos.XPos][currentPos.YPos + 1] && grid[currentPos.XPos][currentPos.YPos + 1] == int.MaxValue)
            grid[currentPos.XPos][currentPos.YPos + 1] = nextCost;
        
        if (currentPos.YPos - 1 >= 0 && !exploredGrid[currentPos.XPos][currentPos.YPos - 1] && grid[currentPos.XPos][currentPos.YPos - 1] == int.MaxValue)
            grid[currentPos.XPos + 1][currentPos.YPos - 1] = nextCost;
    }
    
    private static currentNode MoveToNextNode(int[][] grid, bool[][] exploredGrid, currentNode currentPos, int moveCost)
    {
        var minNode = grid[currentPos.XPos][currentPos.YPos] + moveCost;
        var xMin = 0;
        var yMin = 0;
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                if (grid[i][j] <= minNode && !exploredGrid[i][j])
                {
                    minNode = grid[i][j];
                    xMin = i;
                    yMin = j;
                }   
            }
        }
        currentPos.XPos = xMin;
        currentPos.YPos = yMin;
        exploredGrid[xMin][yMin] = true;
        return currentPos;
    }

    // Coroutine � utiliser pour impl�menter l'algorithme d' A*
    public IEnumerator AStar()
    {
        // Recovering the environment as a matrix
        var matrix = MatrixFromRaycast.CreateMatrixFromRayCast();
        
        /*
        // Convert as boolean grid
        // true => wall || explored
        // false => unknow
        */
        bool[][] ExploredGrid = new bool[matrix.Length][];
        for (int i = 0; i < matrix.Length; i++)
        {
            ExploredGrid[i] = new bool[matrix[i].Length];
            for (int j = 0; j < matrix[i].Length; j++)
            {
                ExploredGrid[i][j]  = (matrix[i][j] == LayerMask.NameToLayer("Obstacle")) ? true : false;
            }
        }
        
        //Label all the nodes with an infinite score
        int[][] Grid = new int[matrix.Length][];
        for (int i = 0; i < matrix.Length; i++)
        {
            Grid[i] = new int[matrix[i].Length];
            for (int j = 0; j < matrix[i].Length; j++)
            {
                Grid[i][j] = int.MaxValue;
            }
        }

        int[][] HeuristicGrid = new int[matrix.Length][];
        for (int i = 0; i < matrix.Length; i++)
        {
            HeuristicGrid[i] = new int[matrix[i].Length];
            for (int j = 0; j < matrix[i].Length; j++)
            {
                HeuristicGrid[i][j] = ManhattanScore(i, j);
            }
        }
        
        // get position
        var startPosX = PlayerScript.StartXPositionInMatrix;
        var startPosY = PlayerScript.StartYPositionInMatrix;
        var endPosX = PlayerScript.GoalXPositionInMatrix;
        var endPosY = PlayerScript.GoalYPositionInMatrix;
        
        // init start pos at 0
        Grid[startPosX][startPosY] = 0;
        ExploredGrid[startPosX][startPosY] = true;
        
        //define move cost
        const int moveCost = 1;
        
        //define the current case
        var currentPos = new currentNode
        {
            XPos = startPosX,
            YPos = startPosY
        };

        var it = 0;

        while (true)
        {
            if (currentPos.XPos == endPosX && currentPos.YPos == endPosY)
            {
                Debug.Log("Solution found in "+it+" iterations");
                //todo return the optimal path
                break;
            }
            MarkNeighbourNode(Grid, ExploredGrid, currentPos, moveCost);
            currentPos = MoveToNextNodeWithHeuristic(Grid, HeuristicGrid, ExploredGrid, currentPos);
            
            it++;
        }
        
        yield return null;
    }

    private static currentNode MoveToNextNodeWithHeuristic(int[][] grid, int[][] heuristicGrid, bool[][] exploredGrid, currentNode currentPos)
    {
        var minNode = Mathf.Abs(grid[0][0] - heuristicGrid[0][0]);
        var xMin = 0;
        var yMin = 0;
        for (var i = 0; i < grid.Length; i++)
        {
            for (var j = 0; j < grid[i].Length; j++)
            {
                if (Mathf.Abs(grid[i][j] - heuristicGrid[i][j]) < minNode && !exploredGrid[i][j] && grid[i][j] != int.MaxValue)
                {
                    minNode = Mathf.Abs(grid[i][j] - heuristicGrid[i][j]);
                    xMin = i;
                    yMin = j;
                }   
            }
        }
        currentPos.XPos = xMin;
        currentPos.YPos = yMin;
        exploredGrid[xMin][yMin] = true;
        return currentPos;
    }
    
    private static int ManhattanScore(int x, int y)
    {
        return (Mathf.Abs(PlayerScript.GoalXPositionInMatrix - x) + 
                Mathf.Abs(PlayerScript.GoalYPositionInMatrix - y)) * 100;
    }

    // Coroutine � utiliser pour impl�menter l'algorithme du recuit simul�
    public IEnumerator SimulatedAnnealing()
    {
        _isRunning = true;

        var currentSolution = new PathSolutionScript(42);

        var scoreEnumerator = GetError(currentSolution);
        yield return StartCoroutine(scoreEnumerator);
        
        float currentError = scoreEnumerator.Current;
        var temperature = 0f;
        var stagnation = 0;

        var minimumError = GetMinError();

        Debug.Log(currentError);

        int iterations = 0;

        while (currentError != minimumError)
        {
            float oldError = currentError;
            
            var newsolution = CopySolution(currentSolution);
            RandomChangeInSolution(newsolution);
            var newscoreEnumerator = GetError(newsolution);
            yield return StartCoroutine(newscoreEnumerator);
            float newError = newscoreEnumerator.Current;

            Debug.Log(currentError + "   -   " + newError);
            var rdm = Random.Range(0f,1f);
            if (rdm <= MetropolisCriterium(currentError,newError,temperature))
            {
                currentSolution = newsolution;
                currentError = newError;
            }
            
            if(oldError == currentError) {
                stagnation++;
            }
            else
            {
                stagnation = 0;
            }
            if(stagnation > 2500) {
                temperature = 6f;
                stagnation = 0;
            }
            
            temperature *= 0.9999f;
            Debug.Log("Temperature: "+temperature+" Stagnation: "+stagnation);
            iterations++;
            yield return 0;
        }

        _isRunning = false;

        Debug.Log("CONGRATULATIONS !!! Solution Found in " + iterations + " iterations !");
    }
    
    float MetropolisCriterium(float currentError, float newError, float temperature) {
        if(temperature <= 0) {
            return currentError - newError >=0 ? 1f: 0f;
        }
        return Mathf.Exp((currentError - newError) / temperature);
    }

    // Coroutine � utiliser pour impl�menter un algorithme g�n�tique
    public IEnumerator GeneticAlgorithm()
    {
        _isRunning = true;
        
        const int popSize = 200;
        const float breedersPercentage = 0.2f;
        var breedersCount = (int) Mathf.Floor(popSize * breedersPercentage);
        var mutationRate = 0.1f;
        var generationId = 0;
        var minError = GetMinError();
        
        // first population initialisation
        Debug.Log("first population initialisation");
        var population = new List<PathSolutionScript>();
        for (var i = 0; i < popSize; i++)
        {
            var pathSize = Random.Range(0, 123);
            population.Add(new PathSolutionScript(pathSize));
        }
        
        while (true)
        {
            generationId++;
            Debug.Log("start population evaluation population n°"+generationId);
            var evaluatedPopulation = new Dictionary<PathSolutionScript, float>(popSize);
            for (var i = 0; i < popSize; i++)
            {
                var scoreEnumerator = GetError(population[i]);
                yield return StartCoroutine(scoreEnumerator);
                var error = scoreEnumerator.Current;
                evaluatedPopulation.Add(population[i], error);
            }

            Debug.Log("evaluation ended !");

            // split selected individu
            Debug.Log("Apply selection");
            var breeders =
                evaluatedPopulation
                    .OrderBy(kv => kv.Value)
                    .Take(breedersCount)
                    .Select(kv => kv.Key)
                    .ToArray();

            //check if a child is the solution
            var newScoreEnumerator = GetError(breeders[0]);
            yield return StartCoroutine(newScoreEnumerator);
            var newError = newScoreEnumerator.Current;
            if (newError <= minError)
            {
                Debug.Log("<color=green>SOLUTION FOUND IN GENERATION " + generationId + "!!!</color>");
                break;
            }

            Debug.Log("<color=blue>Best solution: " + newError + " found in generation: " + generationId + "</color>");
            Debug.Log("selected population size: " + breeders.Length + " at generation: " + generationId);

            //generate cross population
            var newPop = new List<PathSolutionScript>();
            for (var i = 0; i < popSize; i++)
            {
                //select 2 random parents
                var p1 = breeders[Random.Range(0, breeders.Length)];
                var p2 = breeders[Random.Range(0, breeders.Length)];

                var child = CrossOver(p1, p2);
                newPop.Add(child);
            }

            foreach (var individu in newPop)
            {
                var randomValue = Random.Range(0f,1f);
                if(randomValue < mutationRate) {
                    individu.Actions[(int)Random.Range(0, individu.Actions.Length)] = new ActionSolutionScript();
                }
            }
            
            population = newPop;
            yield return null;
        }

        _isRunning = false;
    }

    private static PathSolutionScript CrossOver(PathSolutionScript p1, PathSolutionScript p2)
    {
        var child = p1.Actions.Length > p2.Actions.Length ? new PathSolutionScript(p1.Actions.Length) : new PathSolutionScript(p2.Actions.Length);
        for (var i = 0; i < child.Actions.Length; i++)
        {
            if (i < p1.Actions.Length && i % 2 == 0)
            {
                child.Actions[i] = p1.Actions[i];
            }
            else if (i < p2.Actions.Length && i % 2 != 0)
            {
                child.Actions[i] = p2.Actions[i];
            }
            else if (i > p1.Actions.Length)
            {
                child.Actions[i] = p2.Actions[i];
            }
            else if (i > p2.Actions.Length)
            {
                child.Actions[i] = p1.Actions[i];
            }
        }
        return child;
    }

    /// <summary>
    /// Exemple d'erreur minimum (pas forc�ment toujours juste) renvoyant
    /// la distance de manhattan entre la case d'arriv�e et la case de d�part.
    /// </summary>
    /// <returns></returns>
    int GetMinError()
    {
        return (Mathf.Abs(PlayerScript.GoalXPositionInMatrix - PlayerScript.StartXPositionInMatrix) +
            Mathf.Abs(PlayerScript.GoalYPositionInMatrix - PlayerScript.StartYPositionInMatrix));
    }

    /// <summary>
    /// Exemple d'oracle nous renvoyant un score que l'on essaye de minimiser
    /// Ici est utilis� la position de la case d'arriv�e, la position finale
    /// atteinte par la solution. Il est recommand� d'essayer plusieurs oracles
    /// pour �tudier le comportement des algorithmes selon la qualit� de ces
    /// derniers
    /// 
    /// Parmi les param�tres pouvant �tre utilis�s pour calculer le score/erreur :
    /// 
    ///  - position de la case d'arriv�e    : PlayerScript.GoalXPositionInMatrix
    ///                                       PlayerScript.GoalYPositionInMatrix
    ///  - position du joueur               : player.PlayerXPositionInMatrix
    ///                                       player.PlayerYPositionInMatrix
    ///  - position de d�part du joueur     : PlayerScript.StartXPositionInMatrix
    ///                                       PlayerScript.StartYPositionInMatrix
    ///  - nombre de cases explor�es        : player.ExploredPuts
    ///  - nombre d'actions ex�cut�es       : player.PerformedActionsNumber
    ///  - vrai si le la balle a touch� la case d'arriv�e : player.FoundGoal
    ///  - vrai si le la balle a touch� un obstacle : player.FoundObstacle
    ///  - interrogation de la matrice      :
    ///       � la case de coordonn�e (i, j) est elle un obstacle (i et j entre 0 et 49) :
    ///           player.GetPutTypeAtCoordinates(i, j) == LayerMask.NameToLayer("Obstacle")
    ///       � la case de coordonn�e (i, j) est elle explor�e (i et j entre 0 et 49) :
    ///           player.GetPutTypeAtCoordinates(i, j) == 1
    ///       � la case de coordonn�e (i, j) est elle inexplor�e (i et j entre 0 et 49) :
    ///           player.GetPutTypeAtCoordinates(i, j) == 0
    /// </summary>
    /// <param name="solution"></param>
    /// <returns></returns>
    IEnumerator<float> GetError(PathSolutionScript solution)
    {
        // On indique que l'on s'appr�te � lancer la simulation
        _inSimulation = true;

        // On cr�� notre objet que va ex�cuter notre s�quence d'action
        var player = PlayerScript.CreatePlayer();

        // Pour pouvoir visualiser la simulation (moins rapide)
        player.RunWithoutSimulation = true;

        // On lance la simulation en sp�cifiant
        // la s�quence d'action � ex�cuter
        player.LaunchSimulation(solution);

        // Tout pendant que la simulation n'est pas termin�e
        while (player.InSimulation)
        {
            // On rend la main au moteur Unity3D
            yield return -1f;
        }

        // Calcule la distance de Manhattan entre la case d'arriv�e et la case finale de
        // notre objet, la pond�re (la multiplie par z�ro si le but a �t� trouv�) 
        // et ajoute le nombre d'actions jou�es
        var error = (Mathf.Abs(PlayerScript.GoalXPositionInMatrix - player.PlayerXPositionInMatrix)
            + Mathf.Abs(PlayerScript.GoalYPositionInMatrix - player.PlayerYPositionInMatrix))
            * (player.FoundGoal ? 0 : 100) +
            player.PerformedActionsNumber;

        //Debug.Log(player.FoundGoal);

        // D�truit  l'objet de la simulation
        Destroy(player.gameObject);

        // Renvoie l'erreur pr�c�demment calcul�e
        yield return error;

        // Indique que la phase de simulation est termin�e
        _inSimulation = false;
    }

    /// <summary>
    /// Execute un changement al�atoire sur une solution
    /// ici, une action de la s�quence est tir�e au hasard et remplac�e
    /// par une nouvelle au hasard.
    /// </summary>
    /// <param name="sol"></param>
    public void RandomChangeInSolution(PathSolutionScript sol)
    {
        sol.Actions[Random.Range(0, sol.Actions.Length)] = new ActionSolutionScript();
    }

    /// <summary>
    /// Fonction utilitaire ayant pour but de copier
    /// dans un nouvel espace m�moire une solution
    /// </summary>
    /// <param name="sol">La solution � copier</param>
    /// <returns>Une copie de la solution</returns>
    public PathSolutionScript CopySolution(PathSolutionScript sol)
    {
        // Initialisation de la nouvelle s�quence d'action
        // de la m�me longueur que celle que l'on souhaite copier
        var newSol = new PathSolutionScript(sol.Actions.Length);

        // Pour chaque action de la s�quence originale,
        // on copie le type d'action.
        for (int i = 0; i < sol.Actions.Length; i++)
        {
            newSol.Actions[i].Action = sol.Actions[i].Action;
        }

        // Renvoi de la solution copi�e
        return newSol;
    }
}


