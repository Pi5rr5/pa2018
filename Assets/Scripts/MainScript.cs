using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
                StartCoroutine("NaiveLocalSearch");
            }
        }

        // Affiche un bouton permettant le lancement de la recherche locale na�ve
        if (GUILayout.Button("DEMARRAGE RECUIT SIMULE"))
        {
            // Le bouton est inactif si un algorithme est en cours d'ex�cution
            if (!_isRunning)
            {
                // D�marrage du recuit simul� en pseudo asynchrone
                StartCoroutine("SimulatedAnnealing");
            }
        }

        // Affiche un bouton permettant le lancement de l'algorithme g�n�tique
        if (GUILayout.Button("DEMARRAGE ALGORITHME GENETIQUE"))
        {
            // Le bouton est inactif si un algorithme est en cours d'ex�cution
            if (!_isRunning)
            {
                // D�marrage de l'algorithme g�n�tique en pseudo asynchrone
                StartCoroutine("GeneticAlgorithm");
            }
        }

        // Affiche un bouton permettant le lancement de l'algorithme de Djikstra
        if (GUILayout.Button("DEMARRAGE DJIKSTRA"))
        {
            // Le bouton est inactif si un algorithme est en cours d'ex�cution
            if (!_isRunning)
            {
                // D�marrage de l'algorithme de Djikstra en pseudo asynchrone
                StartCoroutine("Djikstra");
            }
        }

        // Affiche un bouton permettant le lancement de l'algorithme A*
        if (GUILayout.Button("DEMARRAGE A*"))
        {
            // Le bouton est inactif si un algorithme est en cours d'ex�cution
            if (!_isRunning)
            {
                // D�marrage de l'algorithme A* en pseudo asynchrone
                StartCoroutine("AStar");
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
        while (currentError != GetMinError())
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

    // Coroutine � utiliser pour impl�menter l'algorithme de Djikstra
    public IEnumerator Djikstra()
    {
        // R�cup�ration de l'environnement sous forme de matrice
        var matrix = MatrixFromRaycast.CreateMatrixFromRayCast();

        bool[][] booleanGrid = new bool[matrix.Length][];

        // Conversion de la grille propos�e par le probl�me en grille bool�enne (case vide / obstacle)
        for (int i = 0; i < matrix.Length; i++)
        {
            booleanGrid[i] = new bool[matrix[i].Length];
            for (int j = 0; j < matrix[i].Length; j++)
            {
                booleanGrid[i][j] = (matrix[i][j] == LayerMask.NameToLayer("Obstacle")) ? true : false;
            }
        }

        // R�cup�ration des positions de d�part et d'arriv�e
        var startPosX = PlayerScript.StartXPositionInMatrix;
        var startPosY = PlayerScript.StartYPositionInMatrix;
        var endPosX = PlayerScript.GoalXPositionInMatrix;
        var endPosY = PlayerScript.GoalYPositionInMatrix;

        yield return null;
    }

    // Coroutine � utiliser pour impl�menter l'algorithme d' A*
    public IEnumerator AStar()
    {
        //TODO
        yield return null;
    }

    // Coroutine � utiliser pour impl�menter l'algorithme du recuit simul�
    public IEnumerator SimulatedAnnealing()
    {
        //TODO
        yield return null;
    }

    // Coroutine � utiliser pour impl�menter un algorithme g�n�tique
    public IEnumerator GeneticAlgorithm()
    {
        //TODO
        yield return null;
    }

    /// <summary>
    /// Exemple d'erreur minimum (pas forc�ment toujours juste) renvoyant
    /// la distance de manhattan entre la case d'arriv�e et la case de d�part.
    /// </summary>
    /// <returns></returns>
    int GetMinError()
    {
        return (int)(Mathf.Abs(PlayerScript.GoalXPositionInMatrix - PlayerScript.StartXPositionInMatrix) +
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
        player.RunWithoutSimulation = false;

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

        Debug.Log(player.FoundGoal);

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


