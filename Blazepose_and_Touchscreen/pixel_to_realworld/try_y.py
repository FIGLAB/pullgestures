import numpy as np
import cv2
from scipy.interpolate import interp1d
import matplotlib.pyplot as plt

# bot cam
# measured at 7.5 cm zdist away
ys = [0.5770764946937561, 0.624933660030365, 0.6714627838134766, 0.7123582363128662, 0.7515438103675842, 0.7926002740859985, 0.8279182410240173, 0.8587551307678223]
dists = [5, 3, 1, -1, -3, -5, -7, -9]

#top cam
# measured at 12 cm zdist away
# ys = [0.27391868829727173, 0.3342598080635071, 0.3886343240737915, 0.451821506023407, 0.5036942958831787, 0.5516012907028198, 0.6277080774307251]
# dists = [9, 7, 5, 3, 1, -1, -3]
fx = interp1d(ys, dists, fill_value="extrapolate")

xnew = np.linspace(0.5, 0.9, 40)
plt.plot(ys, dists, 'o', xnew, fx(xnew), '-')
plt.show()

# mtx = np.array([[174.44511794, 0.0, 306.20466919],
#             [0.0, 174.81333548, 270.2378873 ],
#             [0.0, 0.0, 1.0]])
