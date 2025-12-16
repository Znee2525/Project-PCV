class EMAFilter:
    def __init__(self, alpha=0.3):
        self.alpha = alpha
        self.prev = {}

    def apply(self, joints):
        """
        joints : dict[str, np.array]
        """
        smoothed = {}

        for k, v in joints.items():
            if k not in self.prev:
                self.prev[k] = v
            self.prev[k] = self.alpha * v + (1 - self.alpha) * self.prev[k]
            smoothed[k] = self.prev[k]

        return smoothed
