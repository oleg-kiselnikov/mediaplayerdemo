   M S B A M L   `   `   `  ����J  FSampleDrawFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:SampleDrawFrameSampleDrawFrame  5   	        SampleDrawFrame.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:SampleDrawFrame     5   	   -   5        $��Window��5   	     � Title$  
MainWindow��5	   	   $	��450��5
   	   $	��800��5   	    � 	Resources 5      )   #SampleDrawFrame.OpenMediaFileDialog   
OpenDialog �� 
TargetType  CommonButtonStyle �� Property �� Value��%�   &	        &	        5   
   6$   �� 5   
   " ��6   .�5      ��  � ��$ 2q�5      �� 5       � ��$ 4q�5      �� 5       � ��$ 80��5      5      5      .��5      ]� .+�5   
   o� $	��Top=�6   .@�5      o� .@�5      m� $
��File��6    � DataContext 5      �� *MediaViewModel+.@�5!      m� $
��Open��6    m� Command 5"      �� *FileOpenCommand+5#      5$      5%      �� 5(   
   $	��Top=�6    �� Orientation$ 
Horizontal=�5)      $��Center=�5*      .+�5+      �� .��5,      �� $ 
Horizontal=�6   .+�5-         SampleDrawFrame.SpinEdit     Value 6%   �� *DrawingViewModel.PenWidth+	 �� Mode$	 OneWayToSource=�   SampleDrawFrame.ColorEdit  5.      
   Color
 6&   �� *DrawingViewModel.PenColor+$	 OneWayToSource=�5/      50      �� 51      .��52      �� $ 
Horizontal=�6    53      �� *DrawingViewModel+.+�55      �� $
��Undo��6   #��[ 57      ��56      �� *UndoCommand+57      �� 58      $
��Redo��6   #��[ 5:      ��59      �� *RedoCommand+5:      �� 5;      $��	Clear All��6   #��[ 5=      ��5<      �� *ClearAllCommand+5=      5>      5?      5@      �� 5B   
   $��Bottom=�6   $ 
Horizontal=�5C      $��Center=�5F       5D      �� *MediaViewModel+.+�5H      �� $��
Frame Back��6   #��[ 5J      ��5I      �� *RewindCommand+5J      �� 5L      ��6   �� *PlaybackSwitchCommand+��5M      �� 5N      " ��6    �� BasedOn# [ 5O      .�5P      ��  � ��$
 Play��5Q   !    �� Triggers 5R      �� 5S       �� Binding 6*   �� *	IsPlaying+ �� Value True5T   *   .a�5U   "   ��  � ��$ Pause��5V   )   5W      5Y      5[      �� 5]      $��Frame Forward��6   #��[ 5_      ��5^      �� *FastForwardCommand+5_      5`      � 5b   
   .+�5c      $   SampleDrawFrame.EmbeddedWindow  -   5          Name$ PreviewArea��5c   #      Handle 5d   #   �� *MediaViewModel.Handle+ �� UpdateSourceTrigger$ PropertyChanged=�$	 OneWayToSource=�!   SampleDrawFrame.DrawingArea  5e      -   5          Name$ DrawingArea��5e          Drawing 5f       �� *DrawingViewModel+��5g      �� 5h      "  6   .�5i      ��  �@ Mode$ Default=�5j   !    5k      �� 5l       6*   �� *MediaViewModel.IsOpened+ True5m   *   .a�5n   "   ��  �@ Mode$ Drawing=�5o   )   5p      5r      5t      5u      5x      5y      