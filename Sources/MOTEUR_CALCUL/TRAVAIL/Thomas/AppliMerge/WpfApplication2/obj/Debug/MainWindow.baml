   M S B A M L   `   `   `  ����J  FWpfApplication2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:WpfApplication2WpfApplication2  5   	        WpfApplication2.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:WpfApplication2     5   	   -   5          �Name$  	Principal��5   	    � Title$ Remplissage cube��5   	   $	��540��6"   $	��820��6/    � 
ResizeMode$ NoResize=�6;    � ShowInTaskbar .   � Topmost .  � WindowStartupLocation$ CenterScreen=�6v   ���	#FFCDCDCD.��5	      � $
��Left=�6   $	��214��6&   $��	50,20,0,0q�63   $	��Top=�6F   $	��720��6^   ���	#FFCDCDCD.+�5
   
   � $��Right=�6   $	��150��6+   $��0,200,0,-200q�68   $	��Top=�6N   $	��720��6f   ���	#FFCDCDCD.+�5      �� -   5         ��Name$ Libelle_Espace��5      $��Espace disque :��6,   $
��Left=�6F   $��	53,46,0,0q�6a   $	��Top=�6t   �� 5      -   5         ��Name$ Barre_Selection��5      $
��Left=�6.   $��174,118,0,0q�6I   $	��Top=�6^   $	��506��6v   �� 5      -   5        $ Libelle_Selection��5      $��Selection :��6/   $
��Left=�6E   $��
80,116,0,0q�6`   $	��Top=�6t   � 5      -   5         �Name$ Barre_Espace��5      $
��Left=�60   $��10��6K   $��
174,54,0,0q�6W   $	��Top=�6k   $	��506��6�   ���	#FF0114D3���	#FF707070���	#FFFDFDFD6�   �� 5      -   5        $ Libelle_Emplacement��5      $��Emplacement du cube :��61   $
��Left=�6Q   $��	10,16,0,0q�6l   $	��Top=�6   �� 5      -   5        $ Libelle_Barre��5      $
��Left=�6+   $��
339,63,0,0q�6F   $	��Top=�6Z   $	��224��6r   �� 5      -   5        $ Libelle_Actuel��5      $��Actuel :��6,   $
��Left=�6?   $��26��6Z   $��
174,94,0,0q�6f   $	��Top=�6z   $	��163��6�   �� 5      -	   5        $ Libelle_Souhaite��5      $��Souhaité :��6.   $
��Left=�6C   $��26��6^   $��
555,94,0,0q�6j   $	��Top=�6~   $	��125��6�   �� 5      -
   5        $ Emplacement_Chemin��5      $
��Left=�60   $��27��6K   $��
174,15,0,0q�6W   $	��Top=�6k   $	��506��6�   5      � 5   
   $
��Left=�6   $	��109��6*   $��0,374,0,-333q�67   $	��Top=�6M   $	��720��6e   ���	#FFCDCDCD.+�5      �� -   5        	 ��Name$	 Bouton_Algo_1��5      $��Go��6,   $
��Left=�69   $��
183,84,0,0q�6T   $	��Top=�6h   $��75��6�   6�   �� 5      -   5        $	 Bouton_Algo_2��5      $��Go��6,   $
��Left=�69   $��
448,84,0,0q�6T   $	��Top=�6h   $��75��6�   6�   �� 5      -   5        $ Libelle_Algo_1��5      $��Algorithme  Thomas��6,   $
��Left=�6I   $��
162,50,0,0q�6d   $	��Top=�6x   �� 5      -   5        $ Libelle_Algo_2��5      $��Algorithme Olivier��6,   $
��Left=�6I   $��
432,50,0,0q�6d   $	��Top=�6x   5      �� 5   
   -   5        $ Libelle_Server��5      $��Server :��6(   $
��Left=�6;   $��	98,31,0,0q�6V   $	��Top=�6i   $��1��6�   �� 5   
   -   5        $	 Bouton_Connexion��5      $��	Connexion��6+   $
��Left=�6?   $��176,99,0,93q�6Z   $��77��6o   
 J� RenderTransformOrigin$
 0.56,-0.932&�6z   $��3��6�   �� 5   
   -   5        $ Libelle_Database��5      $��
Database :��6*   $
��Left=�6?   $��	82,62,0,0q�6Z   $	��Top=�6m   $��3��6�   �� 5   
   -   5         ��Name$ Nom_Database��5      $
��Left=�6(   $��
176,62,0,0q�6C    �� TextWrapping$
 Wrap=�6W   $	��Top=�6k   $	��515��6�   $��2��6�   $��26��6�   $
 0.496,1.904&�6�   �� 5   
   -   5        $ 
Nom_Server��5      $
��Left=�6&   $��
176,31,0,0q�6A   $
 Wrap=�6U   $	��Top=�6i   $	��515��6�   $��1��6�   $��26��6�   6�   �� 5    
   -   5        $ 
Label_Cube��5       $��Cube :��6$   $
��Left=�65   $��104,172,0,0q�6P   $	��Top=�6e   �� 5!   
   -   5         ��Name$ 
Liste_Cube��5!      $
��Left=�6'   $��176,176,0,0q�6B   $	��Top=�6W   $	��515��6o   $��19��6{   5"      5#      