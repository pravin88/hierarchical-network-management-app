   M S B A M L   `   `   `  ����d  `Microsoft.Maps.MapControl.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35tRclr-namespace:Microsoft.Maps.MapControl.WPF;assembly=Microsoft.Maps.MapControl.WPFMicrosoft.Maps.MapControl.WPF  5   	    Project422-2   Project422_2.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   `mRclr-namespace:Microsoft.Maps.MapControl.WPF;assembly=Microsoft.Maps.MapControl.WPF     5   	   -   5        $��MainWin��5   >     � Title$  H-NetMan v1.0��6	   $	��600��6   $	��500��6.    � 
ResizeMode$ 	CanResize=�6M    � WindowStartupLocation$ Manual=�6d    � WindowState$ 	Maximized=�6�    � WindowStyle$ ThreeDBorderWindow=�6�    � SizeToContent$ WidthAndHeight=�6�    � 	Resources 5       �� 
TargetType   gradientMenuItem �� Property	 �� Value
 �� 
StartPoint �� EndPoint � Offset � Color��%  &	        �� 5   
   " m�6   .�5
      ��  � ��
	 �   �6,   �� 5       � ��	 5      �� 5      $
 0.5,0&�6*   $ 0.5,1&�6=   ��5      � $	 0.0Q�6+   $ white��68   � 5      $	 1.0Q�6+   $
 gray��68   5      5      5      5      .��5      ]� ��. .+�5   
   o� -   5        $��topbar��5      $	��Top=�6   $��22��62   ��5      �� 5      $ 0.5,1&�6&   $
 0.5,0&�67   .��5      � $
 Gray��6#   $ 0Q�60   � 5      $ White��6#   $ 1Q�61   5      .@�5      m� -   5        $�� _File��5      .@�5       m� -   5        $��_Exit��5       6*   5!      m� 5"      $��_Configuration��6   .@�5#      m� -   5        $��_Load��5#      6*   m� 5$      -   5        $��_Save��5$      6*   m� 5%      -   5        $��Set _Default View��5%      66   m� 5&      -   5        $��Settings��5&      6-   5'      5(      �� 5+   
   -	   5        $��
statusBar1��5+       $��23��6   $��Bottom=�62   $��Bottom=�6J   .@�5,      �� -
   5        $��statusBarText��5,      field165   �� 5-      5.      � 50   
    � ColumnDefinitions 51      �� 52      $��2* �6#   $	��300��6.   �� 53      $��3* �6#    � RowDefinitions 55      �� 56      $��* �6    �� 57      $��30 �6    .+�59      R� -   5        $��leftPaneDeviceTree��59   (   $��2��6   $��0,0,0,0q�6k   
�������$	��200��6�    � ContextMenu 5:      �� 5;      -   5        $��leftPaneContextMenu��5;   "   .@�5<      m� -   5        $�� Add new Peer Device��5<   #   6A   m� 5=      -   5        $��addSlaveMenuItem��5=   #   $�� Add new Slave Device��6;   6Z   m� 5>      -   5        $��editDeviceMenuItem��5>   #   $�� Edit Device��6=   6S   m� 5?      -   5        $��deleteDeviceMenuItem��5?   #   $�� Delete Device��6?   6W   m� 5@      -   5        $�� View Location��5@   #   6;   5A      5C      '   !Microsoft.Maps.MapControl.WPF.Map  5F      -   5        $��locationMap��5F   P   $��1��6   $��2��6$   +   %Microsoft.Maps.MapControl.WPF.MapCore   ScaleVisibility$ Visible=�65      CredentialsProviderT   NMicrosoft.Maps.MapControl.WPF.Design.ApplicationIdCredentialsProviderConverter$F @ApecU4Us8osvdBtm528_22fCL1S7Awq4AbX70QXzyBXdQymZR00k30J6to0BoCcP 6c      Children. 5G      �� -   5        $��addressResultCanvas��5G   2   $	��300��6   $	��250��6%   .+�5H      R� -   5        $��addressResultTree��5H   K    �� Left$ 7��6   
 �� Top$ 5��6/   $	��239��6>   $	��286��6d   ��6�   Z� R� 5I      -   5        $��chooseDeviceLocationTree��5I   K   $ 7��6   $ 5��6/   $	��239��6>   $	��286��6k   ��6�   Z� �� 5J      -   5        $��addressResultCanvasClose��5J   �   $	 277��6   $ 5��6.   $��16��6=   $
��Fill=�6I   $��16��6X   $7��1/Project422-2;component/bin/Debug/image/close.png��6c   6�   ��5K      �� 5L      $ 0.5,1&�6.   $
 0.5,0&�6?   .��5M      � $ white��6+   $ 0Q�69   � 5N      $
 gray��6+   $ 1Q�68   5O      5Q      5R      � 5T      $��0��6   $��1��6    
���   � J� Opacity$
 0.70Q�6C   $��35��6R   $	��Top=�6^    5U      �� 5V      $��1* �6'   �� 5W      $��3* �6'   .+�5Z      ��  �� Orientation$ 
Horizontal=�6   $
��Left=�66   .+�5[      �� 
�������$��18(�6/   $��Center=�6=   $��Center=�6_   .��6z   Zoom:6�   �� 5\      -   5        $��mapZoomControl��5\   3   $	��192��6    � Value$ 1Q�6)    � Maximum$ 20Q�6t    � SmallChange$ 1Q�6�    �� TickPlacement$ BottomRight=�6�    �� AutoToolTipPlacement$ TopLeft=�6�    �� IsSnapToTickEnabled . 6�   5]      �� 5_      $��1��6   $ 
Horizontal=�6-   $��Right=�6F   .+�5`      �� $��Search: ��6   $��18(�6/   
�������$��Center=�6P   $��Center=�6k   �� 5a      -   5        $��addressSearchText��5a      $	��200��67   $��	Australia��6C   $��Right=�6T   $��1��6p    �� MaxLines$ 1��6}   $��LeftToRight=�6�   
�������
�������$��18(�6  $��1q�6  $��2q�6   $��Center=�64  $��Center=�6O  $��1q�6q  ��6�   Z� 6q  �� 5b      -   5        $/��)/Project422-2;component/Images/search.jpg��5b      6O   5c      5e      5g      5h      5j      