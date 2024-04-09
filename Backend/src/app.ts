import express, { Request, Response, NextFunction } from 'express';
import bodyParser from 'body-parser';
import mongoose from 'mongoose';
import dotenv from 'dotenv';

dotenv.config({ path: './config/.env' });

class CustomError extends Error {
  status: number;

  constructor(message: string, status: number) {
    super(message);
    this.status = status;
  }
}

const app = express();

app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

mongoose.connect(process.env.MONGODB_URI!);

mongoose.connection
  .on('connected', () => {
    console.log('Mongoose is connected');
  })
  .on('error', (err) => {
    console.log(
      'An error has occurred while connecting to mongoose. For further details => ' +
      err
    );
  });

app.use((req: Request, res: Response, next: NextFunction) => {
  res.header('Access-Control-Allow-Origin', '*');
  res.header(
    'Access-Control-Allow-Headers',
    'Origin, X-Requested-With, Content-Type, Accept, Authorization'
  );
  if (req.method === 'OPTIONS') {
    res.header('Access-Control-Allow-Methods', 'PUT, POST, PATCH, DELETE');
    return res.status(200).json({
      message: 'Options are working.',
    });
  }
  next();
});

// User Controller Routes
import userRoutes from './routes/user-routes';
app.use('/api/users/', userRoutes);

app.use((req: Request, res: Response, next: NextFunction) => {
  const error = new CustomError('Not found', 404);
  next(error);
});

app.use((error: CustomError, req: Request, res: Response, next: NextFunction) => {
  res.status(error.status || 500).json({
    error: {
      message: error.message,
    },
  });
});

export default app;
