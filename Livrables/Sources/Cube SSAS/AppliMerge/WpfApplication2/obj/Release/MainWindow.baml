   M S B A M L   `   `   `  ���� I  EOptimiseur ODE, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:WpfApplication2WpfApplication2       WpfApplication2.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     /localclr-namespace:WpfApplication2     -     �Name$  	Principal�� � Title$ Remplissage cube��$	��585��$	��820�� � 
ResizeMode$ CanMinimize=� � Topmost .  � WindowStartupLocation$ CenterScreen=����	#FFCDCDCD.��� $
��Left=�$	��214��$��	50,20,0,0q�$	��Top=�$	��720�����	#FFCDCDCD.+�� $��Right=�$��0,193,0,-73q�$	��720�����	#FFCDCDCD.+��� -    ��Name$ Libelle_Barre��$
��Left=�$��
339,63,0,0q�$	��Top=�$	��224���� -   $ Libelle_Souhaite��$ ��Selection espace alloué :��$
��Left=�$��26��$��	17,55,0,0q�$	��Top=��� -   $ label��$��Aggrégat :��$
��Left=�$��91,0,0,0q�$	��Top=��� -   $ Pres_Aggregat��$��Label��$
��Left=�$��	183,0,0,0q�$	��Top=��� -    ��Name$ Bouton_Aggr��$��Suppr. Aggr��$
��Left=�$��26��$��
614,10,0,0q�$	��Top=�$��77�� J� IsHitTestVisible . �� -   $ 	Size_Util��$��Label��$
��Left=�$��
539,55,0,0q�$	��Top=��� -    ��Name$ Size_MB��$
��Left=�$��26��$��
278,56,0,0q�$	��Top=�$��56���� -	   $ Size_GB��$
��Left=�$��26��$��
183,55,0,0q�$	��Top=�$��60���� -
   $ label1��$��GB��$
��Left=�$��
248,56,0,0q�$	��Top=��� -   $ label2��$��MB��$
��Left=�$��
339,55,0,0q�$	��Top=�	 J� RenderTransformOrigin$	 
-0.5,0.192&��� -   $ label5��$��Taille allouée :��$
��Left=�$��
449,55,0,0q�$	��Top=�$	 -0.735,0.519&�� $
��Left=�$	��191��$��0,287,0,-264q�$	��Top=�$	��720�����	#FFCDCDCD
 � ColumnDefinitions
 �� $
��135* ��� $
��586* �.+��� -   $ Bouton_Algo_1��$��
Metropolis��$
��Left=�$��	45,31,0,0q�$	��Top=�$	��108��$��26��$	 -1.074,0.692&�$��1���� -   $ Bouton_Algo_2��$��Matérialisation��$
��Left=�$��	45,62,0,0q�$	��Top=�$	��108��$��28��$��1��$	 0.426,-0.089&��� -   $ Cancel��$��Cancel��$
��Left=�$��	45,95,0,0q�$	��Top=�$	��108��$��1��� -    �Name$ Barre_Progress��$
��Left=�$��13��$��
10,144,0,0q�$	��Top=�$	��681��$��2���� -   $ Lib_Progress��$��Initialisaiton��$
��Left=�$��
10,152,0,0q�$	��Top=�$��Italic$�$��2���� -   $ label4��$*��$Selection algorithme de traitement :��$
��Left=�$��	-26,0,0,0q�$	��Top=�$��2���� -   $ Libelle_Server��$��Server :��$
��Left=�$��	122,8,0,0q�$	��Top=�$��1���� -   $ Bouton_Connexion��$��	Connexion��$
��Left=�$��
176,39,0,0q�$��77��$	 0.56,-0.932&�$��3��$��26��$	��Top=��� -    ��Name$ 
Liste_Cube��$
��Left=�$	��118��$��
176,75,0,0q�$	��Top=�$	��515���� -   $ Libelle_Database_Copy��$��Database / Cube :��$
��Left=�$��	67,75,0,0q�$	��Top=�$��3���� -    ��Name$ 
Nom_Server��$
��Left=�$��
176,10,0,0q� �� TextWrapping$
 Wrap=�$��TextBox��$	��Top=�$	��515��