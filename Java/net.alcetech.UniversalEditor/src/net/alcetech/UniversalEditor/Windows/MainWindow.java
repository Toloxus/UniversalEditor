package net.alcetech.UniversalEditor.Windows;

import java.awt.BorderLayout;
import java.awt.event.*;

import javax.swing.*;

import net.alcetech.Core.*;
import net.alcetech.UserInterface.*;
import net.alcetech.UserInterface.Controls.*;
import net.alcetech.UserInterface.Theming.ThemeManager;

public class MainWindow extends Window implements ActionListener
{
	private Ribbon ribbon = new Ribbon();

	private JMenuItem mnuFileExit = new JMenuItem();
	
	private void InitializeComponent()
	{
		this.add(this.ribbon);
		
		this.setIconImages(ThemeManager.GetThemedIconImages("MainIcon"));
		this.setSize(800, 600);
		this.setTitle("Universal Editor");
		this.setLayout(new BorderLayout());
		
		JMenu mnuFile = new JMenu();
		mnuFile.setText("File");
		mnuFile.setMnemonic('F');
		mnuFileExit.addActionListener(this);
		mnuFileExit.setText("Exit");;
		mnuFileExit.setMnemonic('x');
		mnuFileExit.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_Q, KeyEvent.CTRL_DOWN_MASK));
		mnuFile.add(mnuFileExit);
		
		CommandBar cbMenuBar = new CommandBar();
		cbMenuBar.add(mnuFile);
		this.add(cbMenuBar, BorderLayout.NORTH);
	}
	
	public MainWindow()
	{
		InitializeComponent();
	}
	
	protected void OnClosing(CancelEventArgs e)
	{
		MessageDialogResult result = MessageDialog.ShowDialog(this, "You have unsaved changes. Do you wish to save your changes before closing this window?", "Close Program", MessageDialogButtons.YesNoCancel);
		switch (result)
		{
			case Yes:
			{
				break;
			}
			case No:
			{
				break;
			}
			case Cancel:
			{
				e.cancel();
				break;
			}
		}
	}
	
	public void actionPerformed(ActionEvent evt)
	{
		if (evt.getSource() == mnuFileExit)
		{
			Application.Exit();
		}
	}
}