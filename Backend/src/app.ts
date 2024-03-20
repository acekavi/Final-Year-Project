import express, { Request, Response, NextFunction } from 'express';
import bodyParser from 'body-parser';
import mongoose from 'mongoose';
import dotenv from 'dotenv';
import cors from 'cors';

dotenv.config({ path: './config/.env' });

const app = express();

app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

mongoose.connect(process.env.MONGODB_URI!);

mongoose.connection.on('connected', () => {
    console.log('Mongoose is connected');
}).on('error', (err) => {
    console.log('An error has occurred while connecting to mongoose. For further details => ' + err);
});

app.use((req: Request, res: Response, next: NextFunction) => {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Headers', 'Origin, X-Requested-With, Content-Type, Accept, Authorization');
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
    const error = new Error('Not found') as any; // "as any" is used here to add the status property to the Error object
    error.status = 404;
    next(error);
});

app.use((error: any, req: Request, res: Response, next: NextFunction) => { // "error: any" since Error doesn't have a status property by default
    res.status(error.status || 500).json({
        error: {
            message: error.message,
        },
    });
});

export default app;