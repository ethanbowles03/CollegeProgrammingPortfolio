// Authors: Conner Fisk, Ethan Bowles
// Class that contains the code for handling events and sending what needs to be sent to the server to the GameController.
// Date: Nov 18, 2022

namespace SnakeGame;

public partial class MainPage : ContentPage
{
    // Fields
    private GameController gameController;
    private bool worldSet;

    /// <summary>
    /// Constructor for a MainPage
    /// </summary>
    public MainPage()
    {
        // Initialize the needed components.
        InitializeComponent();

        // Initialize gameController and set where the events should be handeled.
        gameController = new GameController();
        gameController.MessagesArrived += DisplayMessages;
        gameController.Error += ShowError;

        // Call Invalidate on the graphicsView
        graphicsView.Invalidate();

        // Set the world to gameController.GetWorld()
        worldPanel.SetWorld(gameController.GetWorld());
    }

    /// <summary>
    /// Handler for the controller's Error event
    /// </summary>
    /// <param name="err"></param>
    private void ShowError(string err)
    {
        // Show the error
        Dispatcher.Dispatch(() => DisplayAlert("Error", err, "OK"));

        // Then re-enable the controlls so the user can reconnect
        Dispatcher.Dispatch(
          () =>
          {
              connectButton.IsEnabled = true;
              serverText.IsEnabled = true;
          });
    }

    /// <summary>
    /// Handler for the controller's MessagesArrived event
    /// </summary>
    /// <param name="newMessages"></param>
    private void DisplayMessages(IEnumerable<string> newMessages)
    {
        // If the world has not been set...
        if (!worldSet)
        {
            // Set the world with the gameController's world.
            worldPanel.SetWorld(gameController.GetWorld());
            // Get the current player's ID
            int.TryParse(gameController.pId, out int curId);
            // Set the current player's ID
            worldPanel.SetPId(curId);
            // Set the boolean worldSet to true.
            worldSet = true;
        }
        // Call Invalidate on the graphicsView
        graphicsView.Invalidate();
    }

    /// <summary>
    /// Used for the user's controls.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void OnTapped(object sender, EventArgs args)
    {
        keyboardHack.Focus();
    }

    /// <summary>
    /// Method used to handle when the text has been changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void OnTextChanged(object sender, TextChangedEventArgs args)
    {
        // Get the entry.
        Entry entry = (Entry)sender;
        // Get a string value of the entrys text
        String text = entry.Text.ToLower();

        // If the entry's text was "w"
        if (text == "w")
        {
            // Move up
            gameController.MessageEntered("{\"moving\":\"up\"}");
        }
        // If the entry's text was "a"
        else if (text == "a")
        {
            // Move left
            gameController.MessageEntered("{\"moving\":\"left\"}");
        }
        // If the entry's text was "s"
        else if (text == "s")
        {
            // Move down
            gameController.MessageEntered("{\"moving\":\"down\"}");
        }
        // If the entry's text was "d"
        else if (text == "d")
        {
            // Move right
            gameController.MessageEntered("{\"moving\":\"right\"}");
        }
        // Reset the entry's text to nothing.
        entry.Text = "";
    }

    /// <summary>
    /// Used to handle network errors.
    /// </summary>
    private void NetworkErrorHandler()
    {
        // Display an error pop-up with an appropriate message.
        DisplayAlert("Error", "Disconnected from server", "OK");
    }


    /// <summary>
    /// Event handler for the connect button
    /// We will put the connection attempt logic here in the view, instead of the controller,
    /// because it is closely tied with disabling/enabling buttons, and showing dialogs.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void ConnectClick(object sender, EventArgs args)
    {
        // If the user did not enter a server address...
        if (serverText.Text == "")
        {
            // Display a pop-up asking the user to enter an address.
            DisplayAlert("Error", "Please enter a server address", "OK");
            return;
        }
        // If the user did not enter a name...
        if (nameText.Text == "")
        {
            // Display a pop-up asking the user to enter a name.
            DisplayAlert("Error", "Please enter a name", "OK");
            return;
        }
        // If the user entered a name longer than 16 characters...
        if (nameText.Text.Length > 16)
        {
            // Display a pop-up asking the user to enter a name less than 16 characters.
            DisplayAlert("Error", "Name must be less than 16 characters", "OK");
            return;
        }
        // Call Connect on gameController with the server address and name entered.
        gameController.Connect(serverText.Text,nameText.Text);
        // Call Focus on keyboardHack
        keyboardHack.Focus();
    }

    /// <summary>
    /// Used when the controls button has been clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ControlsButton_Clicked(object sender, EventArgs e)
    {
        // Display an alert showing what the controls are and what they do.
        DisplayAlert("Controls",
                     "W:\t\t Move up\n" +
                     "A:\t\t Move left\n" +
                     "S:\t\t Move down\n" +
                     "D:\t\t Move right\n",
                     "OK");
    }

    /// <summary>
    /// Used when the about button has been clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AboutButton_Clicked(object sender, EventArgs e)
    {
        // Dispaly an alert showing an "About" description for the game.
        DisplayAlert("About",
      "SnakeGame solution\nArtwork by Jolie Uk and Alex Smith\nGame design by Daniel Kopta and Travis Martin\n" +
      "Implementation by Conner Fisk & Ethan Bowles\n" +
        "CS 3500 Fall 2022, University of Utah", "OK");
    }

    /// <summary>
    /// Used to show the focus.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ContentPage_Focused(object sender, FocusEventArgs e)
    {
        // If connectButton is not enabled, call Focus on keyboardHack
        if (!connectButton.IsEnabled)
            keyboardHack.Focus();
    }
}